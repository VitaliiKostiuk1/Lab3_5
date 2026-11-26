@echo off
chcp 65001 > nul
REM Запуск тестів зі збором покриття та генерацією HTML-звіту.
REM Потрібно один раз встановити генератор звітів:
REM   dotnet tool install -g dotnet-reportgenerator-globaltool

if exist TestResults rmdir /s /q TestResults
if exist CoverageReport rmdir /s /q CoverageReport

dotnet test Lab3_5.sln --collect:"XPlat Code Coverage" --settings coverage.runsettings --results-directory TestResults
if errorlevel 1 goto :eof

reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:"Html;TextSummary"
type CoverageReport\Summary.txt
start CoverageReport\index.html
