#!/bin/bash
set -e

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${GREEN}=========================================="
echo "Running tests with code coverage"
echo -e "==========================================${NC}"

# Get the directory where this script is located (solution root)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo -e "${YELLOW}📁 Solution root: $(pwd)${NC}"

# Check if coverlet.runsettings exists in the current directory
if [ ! -f "coverlet.runsettings" ]; then
    echo -e "${RED}❌ Error: coverlet.runsettings not found in $(pwd)${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Using $(pwd)/coverlet.runsettings${NC}"

# Find the test project directory
if [ -d "ZeroTrustAPI.Tests" ]; then
    TEST_PROJECT="ZeroTrustAPI.Tests"
elif [ -d "tests/ZeroTrustAPI.Tests" ]; then
    TEST_PROJECT="tests/ZeroTrustAPI.Tests"
else
    echo -e "${RED}❌ Error: Could not find ZeroTrustAPI.Tests folder${NC}"
    exit 1
fi

echo -e "${YELLOW}📁 Test project: $TEST_PROJECT${NC}"

# Run tests from the solution root, specifying the test project
echo -e "${GREEN}🔍 Running dotnet test with coverage...${NC}"
dotnet test "$TEST_PROJECT" --collect:"XPlat Code Coverage" --settings coverlet.runsettings --results-directory "$TEST_PROJECT/TestResults"

# Find coverage file
COVERAGE_FILE=$(find "$TEST_PROJECT/TestResults" -name "coverage.cobertura.xml" -type f | head -1)

if [ -z "$COVERAGE_FILE" ]; then
    echo -e "${RED}❌ Error: coverage.cobertura.xml not found${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Coverage file found: $COVERAGE_FILE${NC}"

# Generate HTML report
echo -e "${GREEN}📊 Generating HTML coverage report...${NC}"
reportgenerator "-reports:$COVERAGE_FILE" "-targetdir:$TEST_PROJECT/CoverageReport" -reporttypes:Html

echo -e "${GREEN}=========================================="
echo "✅ Coverage report generated at:"
echo "   $TEST_PROJECT/CoverageReport/index.html"
echo -e "==========================================${NC}"

# Open report if not in CI
if [ -z "$CI" ]; then
    if command -v open &> /dev/null; then
        open "$TEST_PROJECT/CoverageReport/index.html"
    elif command -v xdg-open &> /dev/null; then
        xdg-open "$TEST_PROJECT/CoverageReport/index.html"
    elif command -v start &> /dev/null; then
        start "$TEST_PROJECT/CoverageReport/index.html"
    else
        echo -e "${YELLOW}Open the report manually from the path above.${NC}"
    fi
fi