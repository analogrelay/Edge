@echo off
echo When running:
echo Press ESC to recompile and restart
echo Close this window to stop
echo ...

ping localhost -n 2 >NUL


:top
call build start
goto top
