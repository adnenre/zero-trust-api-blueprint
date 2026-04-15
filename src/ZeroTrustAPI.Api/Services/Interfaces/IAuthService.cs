using System.Threading.Tasks;
using ZeroTrustAPI.Api.DTOs;

namespace ZeroTrustAPI.Api.Services.Interfaces;

/// <summary>
/// Defines the contract for authentication-related operations.
/// <para>
/// OOP Principle: <strong>Abstraction</strong> – This interface hides the implementation details 
/// of registration, login, token refresh, and revocation. It provides a clean, high-level contract 
/// that the rest of the application can depend on.
/// </para>
/// <para>
/// OOP Principle: <strong>Dependency Inversion (SOLID – D)</strong> – High-level modules (e.g., 
/// AuthController) depend on this abstraction, not on concrete AuthService implementations. 
/// This makes the system loosely coupled and testable.
/// </para>
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user with the provided credentials.
    /// </summary>
    /// <param name="request">Registration data (username, email, password, etc.).</param>
    /// <returns>An <see cref="AuthResult"/> containing the outcome (success/failure, tokens, errors).</returns>
    /// <remarks>
    /// OOP Concepts:
    /// <list type="bullet">
    /// <item><description><strong>Polymorphism</strong> – Different implementations of IAuthService can provide different registration logic (e.g., email verification, social login).</description></item>
    /// <item><description><strong>Encapsulation</strong> – The interface does not expose how passwords are hashed or how users are stored; it only declares what happens.</description></item>
    /// </list>
    /// </remarks>
    Task<AuthResult> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates an existing user and returns access/refresh tokens.
    /// </summary>
    /// <param name="request">Login credentials (username/email and password).</param>
    /// <returns>An <see cref="AuthResult"/> with tokens on success, or errors on failure.</returns>
    /// <remarks>
    /// OOP Concepts:
    /// <list type="bullet">
    /// <item><description><strong>Contract</strong> – The method signature defines what inputs are required and what output to expect, without specifying the authentication mechanism (JWT, cookies, etc.).</description></item>
    /// <item><description><strong>Separation of Concerns</strong> – Authentication logic is isolated from the controller, promoting single responsibility.</description></item>
    /// </list>
    /// </remarks>
    Task<AuthResult> LoginAsync(LoginRequest request);

    /// <summary>
    /// Generates a new access token using a valid refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token previously issued during login or registration.</param>
    /// <returns>An <see cref="AuthResult"/> containing a new access token (and optionally a new refresh token).</returns>
    /// <remarks>
    /// OOP Concepts:
    /// <list type="bullet">
    /// <item><description><strong>Interface Segregation (SOLID – I)</strong> – The IAuthService interface is focused only on authentication; refreshing tokens is a cohesive part of the same abstraction.</description></item>
    /// <item><description><strong>Liskov Substitution (SOLID – L)</strong> – Any derived class implementing RefreshTokenAsync must be substitutable for the base abstraction without altering correctness.</description></item>
    /// </list>
    /// </remarks>
    Task<AuthResult> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Revokes all refresh tokens belonging to a specific user (e.g., on logout or password change).
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose tokens should be revoked.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// OOP Concepts:
    /// <list type="bullet">
    /// <item><description><strong>Encapsulation</strong> – The interface does not reveal how tokens are stored (in‑memory, database, cache).</description></item>
    /// <item><description><strong>Single Responsibility</strong> – This method specifically handles token revocation, keeping the interface cohesive.</description></item>
    /// </list>
    /// </remarks>
    Task RevokeRefreshTokensAsync(Guid userId);
}