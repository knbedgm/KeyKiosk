REM https://stackoverflow.com/questions/7044985/how-can-i-auto-elevate-my-batch-file-so-that-it-requests-from-uac-administrator
REM https://stackoverflow.com/questions/8797983/can-a-windows-batch-file-determine-its-own-file-name
REM https://old.reddit.com/r/msp/comments/m215uh/local_group_policies_using_lgpo_lgpoexe_to_deploy/
REM https://www.elevenforum.com/t/how-to-install-group-policy-templates-for-edge.23597/

@ECHO OFF & CLS & ECHO.
NET FILE 1>NUL 2>NUL & IF ERRORLEVEL 1 (ECHO Admin rights required. Elevating... & powershell -command "Start-Process cmd -ArgumentList '/c cd /d %CD% && %~nx0' -Verb runas"& EXIT /B)
REM ... proceed here with admin rights ...

cd ApplyEdgePolicy
LGPO.exe /t EdgeKioskPolicy.gpo.txt
pause