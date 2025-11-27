@echo off
cls
echo ========================================
echo   Avvio GiacenzaSorterRm - TestDev
echo   (Ambiente CON LDAP/Active Directory)
echo ========================================
echo.
echo Configurazione:
echo   - Database: Server Test Remoto
echo   - Active Directory: ABILITATO (test-dc.postel.it)
echo   - Utenti ESTERNI + AD
echo   - Lockout: 5 tentativi / 10 minuti
echo.
echo ========================================

REM Imposta ambiente
set ASPNETCORE_ENVIRONMENT=TestDev

echo.
echo [INFO] Environment impostato: %ASPNETCORE_ENVIRONMENT%
echo.

REM Verifica User Secrets configurati
echo [INFO] Verifica User Secrets...
dotnet user-secrets list | findstr "ConnectionStrings" >nul
if errorlevel 1 (
    echo.
    echo [WARN] User Secrets non configurati!
    echo.
    echo Esegui prima:
    echo   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=TEST-SQL-SERVER;Database=GIACENZA_SORTER_RM_TEST;User Id=test_user;Password=test_password;TrustServerCertificate=True;"
    echo   dotnet user-secrets set "ActiveDirectory:ServiceAccount:Username" "POSTEL\svc_giacenza_test"
    echo   dotnet user-secrets set "ActiveDirectory:ServiceAccount:Password" "YOUR_PASSWORD"
    echo.
    pause
    exit /b 1
)

echo [OK] User Secrets configurati
echo.

REM Verifica AD Secrets per TestDev
dotnet user-secrets list | findstr "ActiveDirectory" >nul
if errorlevel 1 (
    echo.
    echo [WARN] Active Directory secrets non configurati!
    echo.
    echo Esegui:
    echo   dotnet user-secrets set "ActiveDirectory:ServiceAccount:Username" "POSTEL\svc_giacenza_test"
    echo   dotnet user-secrets set "ActiveDirectory:ServiceAccount:Password" "YOUR_PASSWORD"
    echo.
    pause
    exit /b 1
)

echo [OK] Active Directory secrets configurati
echo.

REM Avvia applicazione
echo [INFO] Avvio applicazione...
echo.
dotnet run

pause
