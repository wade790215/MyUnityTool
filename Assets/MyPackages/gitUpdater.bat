set projectPath=%cd%
cd /d %projectPath%

echo "Update all branch to latest develop--------------------------------------"
git checkout develop
git fetch origin
git reset --hard origin/develop
git pull
git submodule foreach --recursive git checkout develop
git submodule foreach --recursive git fetch origin
git submodule foreach --recursive git reset --hard origin/develop
git submodule foreach --recursive git pull

set /p curVersion=<Ver.txt
echo "Current Version %curVersion% --------------------------------------"

set /p version="Input Tag Version :"
echo "Start tag to version %version%--------------------------------------"
echo %version% > Ver.txt

:: === Update package.json version field ===
powershell -Command "(Get-Content package.json) -replace '\"version\":\s*\"[^\"]+\"', '\"version\": \"%version%\"' | Set-Content package.json"
echo "package.json updated to version %version%"

echo "Commit & Push Changed--------------------------------------"
set GIT_MERGE_AUTOEDIT=no

git commit -a -m "update submodule and change version info to %version%"
echo "Commit & Push Changed--------------------------------------"
set GIT_MERGE_AUTOEDIT=no
git commit -a -m "update submodule and change version info to %version%"
git push
echo "Start Release flow--------------------------------------"
git checkout master
git reset --hard origin/master
git pull
git flow release start %version%
git flow release finish -m %version%
git checkout master
git push
git checkout develop
git push --tags
git push
echo "Done--------------------------------------"
pause