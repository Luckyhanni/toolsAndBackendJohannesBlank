# CodeCounter CLI

Die CodeCounter CLI durchsucht ein Verzeichnis rekursiv nach C#-Dateien und erstellt eine Analyse.  
Ergebnisse werden in der Konsole angezeigt und als `code_report.json` gespeichert.

---

## Verwendung

Standard:
dotnet run -- "<Pfad>"

Verbose:
dotnet run -- "<Pfad>" --verbose

Hilfe:
dotnet run -- --help

---

## Optionen

**<Pfad>**  
Pfad zum Ordner, der durchsucht werden soll. Muss in Anführungszeichen stehen, wenn Leerzeichen enthalten sind.

**--verbose**  
Zeigt zusätzlich jede verarbeitete Datei und die Top-5 Dateien mit den meisten Codezeilen.

**--help**  
Listet alle verfügbaren Befehle und deren Nutzung auf.

---

## Ausgabe

- Anzahl gefundener `.cs`-Dateien  
- Gesamtanzahl der Codezeilen (ohne Kommentare und Leerzeilen)  
- Anzahl Klassen und `partial class`  
- Anzahl eindeutiger Klassen  
- Speicherung aller Daten in `code_report.json`

---

## Fehlerfälle

- Ungültiger oder nicht existierender Pfad  
- Keine `.cs`-Dateien gefunden  
- Pfad ohne Anführungszeichen bei Leerzeichen  
- Zugriffsfehler auf Unterordner

---
