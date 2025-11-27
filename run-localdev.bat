@echo off
cls
echo ========================================
echo   Avvio GiacenzaSorterRm - LocalDev
echo   (Ambiente SENZA LDAP/Active Directory)
echo ========================================
echo.
echo Configurazione:
echo   - Database: SQL Express Locale
echo   - Active Directory: DISABILITATO
echo   - Solo utenti ESTERNI (password hash)
echo   - Lockout: 10 tentativi / 5 minuti
echo.
echo ========================================

REM Imposta ambiente
set ASPNETCORE_ENVIRONMENT=LocalDev

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
    echo   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=VEVRFL1M031H;Database=GIACENZA_SORTER_RM;Integrated Security=True;TrustServerCertificate=True;"
    echo.
    pause
    exit /b 1
)

echo [OK] User Secrets configurati
echo.

REM Avvia applicazione
echo [INFO] Avvio applicazione...
echo.
dotnet run

pause
