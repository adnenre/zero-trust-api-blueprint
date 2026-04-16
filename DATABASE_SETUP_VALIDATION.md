# Validation de la configuration PostgreSQL avec EF Core

Ce guide décrit les étapes de validation locale pour s'assurer que la base de données PostgreSQL, les migrations Entity Framework Core et le health check fonctionnent correctement.

## Prérequis

- Docker et Docker Compose installés
- .NET SDK 10.0.100
- Outil `dotnet-ef` installé globalement :

```bash
dotnet tool install --global dotnet-ef
```

## Étapes de validation

### 1. Nettoyer l'environnement existant

Supprime les anciennes migrations et les volumes Docker (base de données) pour repartir d'un état vierge.

```bash
cd src/ZeroTrustAPI.Api
rm -rf Migrations
docker-compose down -v
```

### 2. Créer une nouvelle migration

Génère une migration initiale incluant toutes les entités (`User`, `Role`, `AuditLog`, etc.) ainsi que les nouvelles propriétés (`LastLoginAt`, `IsActive`).

```bash
dotnet ef migrations add InitialCreate
```

### 3. Reconstruire et lancer l'environnement complet

Construit l'image Docker de l'API (sans utiliser le cache) et démarre les conteneurs.

```bash
docker-compose build --no-cache api
docker-compose up -d
```

### 4. Attendre que PostgreSQL soit healthy

Laisse le temps à PostgreSQL de démarrer et de passer le health check.

```bash
sleep 10
```

### 5. Vérifier que l'API répond (health check)

Le endpoint `/health` doit retourner un statut `healthy`.

```bash
curl -f http://localhost:5000/health
```

**Réponse attendue :**

```
{"status":"healthy","timestamp":"2026-04-16T00:19:16.7924244Z"}
```

### 6. Vérifier les tables

Liste toutes les tables de la base `zero_trust`. Les 9 tables attendues + `__EFMigrationsHistory` doivent apparaître.

```bash
docker exec zero-trust-postgres psql -U postgres -d zero_trust -c "\dt"
```

**Extrait attendu :**

```
 public | ApiKeys               | table | postgres
 public | AuditLogs             | table | postgres
 public | Devices               | table | postgres
 public | RefreshTokens         | table | postgres
 public | Roles                 | table | postgres
 public | Sessions              | table | postgres
 public | UploadedFiles         | table | postgres
 public | UserRoles             | table | postgres
 public | Users                 | table | postgres
 public | __EFMigrationsHistory | table | postgres
```

### 7. Vérifier l'absence du warning `UserId1`

Le warning lié à la clé étrangère `AuditLog.UserId1` ne doit plus apparaître.

```bash
docker logs zero-trust-api 2>&1 | grep "UserId1"
```

**Résultat attendu :** Aucune sortie (silence).

### 8. Vérifier les colonnes de la table `Users`

La table `Users` doit contenir les colonnes `lastloginat` et `isactive`.

```bash
docker exec zero-trust-postgres psql -U postgres -d zero_trust -c "\d \"Users\""
```

**Colonnes attendues :**

```
 id           | uuid           | not null
 username     | text           | not null
 email        | text           | not null
 passwordhash | text           | not null
 createdat    | timestamp      | not null
 lastloginat  | timestamp      |
 isactive     | boolean        | not null default true
```

### 9. Tester l'idempotence des migrations

Redémarre l'API ; les migrations ne doivent pas être réappliquées.

```bash
docker restart zero-trust-api
sleep 5
docker logs zero-trust-api --tail 20 | grep -E "No migrations were applied|already up to date"
```

**Message attendu :**

```
No migrations were applied. The database is already up to date.
```

### 10. Simuler l'indisponibilité de PostgreSQL

Arrête PostgreSQL et vérifie que le health check retourne `503 Service Unavailable`.

```bash
docker stop zero-trust-postgres
sleep 2
curl -o /dev/null -s -w "%{http_code}\n" http://localhost:5000/health
```

**Code attendu :** `503`

### 11. Rétablir PostgreSQL

Redémarre PostgreSQL et vérifie que l'API redevient healthy.

```bash
docker start zero-trust-postgres
sleep 5
curl -f http://localhost:5000/health
```

**Résultat attendu :** Retour à `200` avec le JSON de santé.

## Résumé des critères de succès

- ✅ Les 9 tables sont créées.
- ✅ Le health check retourne `200` quand PostgreSQL est opérationnel, `503` quand il est arrêté.
- ✅ Les migrations sont idempotentes (aucune erreur au redémarrage).
- ✅ Aucun warning EF Core (`UserId1`) dans les logs.
- ✅ Les nouvelles colonnes `LastLoginAt` et `IsActive` sont présentes dans la table `Users`.

## Nettoyage (optionnel)

Pour arrêter et supprimer tous les conteneurs et volumes :

```bash
docker-compose down -v
```

---

Ce fichier sert de validation finale avant le commit et le déclenchement du pipeline CI/CD.
