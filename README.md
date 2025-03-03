#CloudGameServer

## Description
A game server that can start your vm and start the servers from just a webbsite.
Bash scripts that starts the servers automaticly when the vm starts

## Script

### Script 1
```bash
#!/bin/bash

# Namn på din screen-session
SCREEN_NAME="armaserver"

# Skicka kommandot "diag" till screen-sessionen
screen -S $SCREEN_NAME -p 0 -X stuff "diag\n"

# Vänta lite för att ge servern tid att svara
sleep 5

# Hämta de senaste 50 raderna från screen-sessionen
screen -S $SCREEN_NAME -p 0 -X hardcopy -h /tmp/armascreen_output.txt
tail -n 20 /tmp/armascreen_output.txt > /tmp/last_20_lines.txt

# Kontrollera om filen skapades
if [ ! -f /tmp/last_20_lines.txt ]; then
    echo "Fel: Ingen output från screen-sessionen."
    exit 1
fi

# Läs utdata från den temporära filen
OUTPUT=$(cat /tmp/last_20_lines.txt)

# Kontrollera om det finns "bandwidth", "ping" och "desync" i utdata
if echo "$OUTPUT" | grep -q "bandwidth=<"; then
    if echo "$OUTPUT" | grep -q "ping=<"; then
        if echo "$OUTPUT" | grep -q "desync="; then
            echo "Spelare är online."
            exit 0
        fi
    fi
fi

echo "Inga spelare online. Stänger ner Arma 3-servern..."
# Stäng ner servern
screen -S $SCREEN_NAME -X quit

```
## Script 2
```Bash
#!/bin/bash

# Starta en screen-session för Arma 3-servern
screen -S armaserver -dm bash -c '
    cd /arma_server_disk/.local/share/Steam/steamcmd/arma3  # Gå till mappen där mod-filerna finns
    MODS=$(ls -d @* | tr "\n" ";")  # Hitta alla mods och formatera dem med semikolon
    ./arma3server_x64 -config=server.cfg -mod="$MODS"
'
```
## Script 3
```bash

#!/bin/bash
sleep 30
# Namnet på din screen-session
SCREEN_NAME="minecraft"

# Kontrollera om screen-sessionen redan är igång
if screen -list | grep -q "$SCREEN_NAME"; then
    echo "Minecraft server is already running in screen session '$SCREEN_NAME'."
else
    # Starta Minecraft-servern i en ny screen-session
    echo "Starting Minecraft server in screen session '$SCREEN_NAME'..."
    cd /home/drstrange/minecraftserver
    screen -dmS "$SCREEN_NAME" bash -c "./start.sh; exec bash"
    echo "Minecraft server started in screen session '$SCREEN_NAME'."
fi
sleep 10
```
## Script 4
```bash
#!/bin/bash

# Ange sökvägen till din Minecraft server
SERVER_DIR="/home/drstrange/minecraftserver"
SCREEN_NAME="minecraft"

# Kontrollera om servern körs i en screen-session
if screen -list | grep -q "$SCREEN_NAME"; then
    # Skicka kommando till Minecraft server för att lista spelare
    screen -S "$SCREEN_NAME" -p 0 -X stuff "list\n"

    # Vänta en stund för att ge servern tid att svara
    sleep 5

    # Kontrollera loggar för att se om några spelare är online
    if grep -q "There are 0 of a max of 10 players online:" "$SERVER_DIR/logs/latest.log"; then
        echo "No players online. Shutting down server."

        # Skicka kommando till Minecraft server för att stänga av den
        screen -S "$SCREEN_NAME" -p 0 -X stuff "stop\n"
        sleep 10

        # Stäng av servern
        if screen -list | grep -q "$SCREEN_NAME"; then
            screen -S "$SCREEN_NAME" -p 0 -X quit
        fi

    fi
else
    echo "Minecraft server is not running."
fi
```
## Script 5
```bash
#!/bin/bash

# Namn på screen-sessioner
MINECRAFT_SCREEN_NAME="minecraft"
ARMA_SCREEN_NAME="armaserver"

# Kontrollera om det finns några aktiva screen-sessioner
SCREEN_OUTPUT=$(screen -list)

# Kontrollera om Minecraft-servern körs
if echo "$SCREEN_OUTPUT" | grep -q "$MINECRAFT_SCREEN_NAME"; then
    echo "Minecraft server is running."
else
    echo "Minecraft server is not running."
fi

# Kontrollera om Arma 3-servern körs
if echo "$SCREEN_OUTPUT" | grep -q "$ARMA_SCREEN_NAME"; then
    echo "Arma 3 server is running."
else
    echo "Arma 3 server is not running."
fi

# Om ingen server är igång, stäng av och deallokera VM:n
if ! echo "$SCREEN_OUTPUT" | grep -q "$MINECRAFT_SCREEN_NAME" && ! echo "$SCREEN_OUTPUT" | grep -q "$ARMA_SCREEN_NAME"; then
    echo "No servers are running. Shutting down VM."
    # Avkommentera raden nedan för att faktiskt stänga av VM:n
    az vm deallocate --resource-group "R.GAMESERVER_GROUP" --name "r.gameserver"
else
    echo "At least one server is running. Keeping VM active."
fi
