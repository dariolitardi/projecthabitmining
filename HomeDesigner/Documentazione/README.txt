Il simulatore HomeDesigner è un software standalone su sistema operativo Windows, scritto in C#. 
Per usarlo e lavorare sull'implementazione consiglio l'IDE Visual Studio.
All'esecuzione del simulatore l'interfaccia utente presenta una serie di comandi, che elenco e spiego di seguito.

1.	Nella parte destra si trovano 3 grandi textbox. "positions" serve per configurare le varie posizioni delle mura 
	dello smart space. Un muro è delimatato da una coppia di posizioni.
2.	Per disegnare un muro basta scrivere in "walls" la coppia di posizione ad es "p1,p2" create in posizions.
3.	Per configurare i sensori basta spuntare la checkbox "add sensors" e sulla mappa si attiva la configurazione dei 
	sensori. I sensori sono PIR sensors, ossia sensori di movimento, configurati nell'implementazione di raggio 1 metro.
4.	Una volta costruita la smart house, scegli un nome e scrivilo nella casella "choose home name for save" e fai save.
5.	La combo box "choose home name for load" ti fa selezionare e caricare tutti gli smart spaces che hai costruito.
6.	Per fare una simulazione clicca simulate, ovviamente dopo aver caricato una casa, scegli il numero di utenti che si muovono,
	il tipo di traiettorie, i giorni di simulazione. Per ogni utente puoi scegliere la velocita, se costante o variabile. Se costante 
	anche il modulo della velocità. In caso di velocità variabile (moto vario), lascia la combo box speed non selezionata.
7.	Le traiettorie simulate vengono salvate in una cartella "...\HomeDesigner\bin\Debug\Log\LogDellaTuaUltimaSimulazione". 
	Qui vengono salvati una serie di log, in formato file di testo ".txt" che contengono le traiettorie di ogni utente 
	ad es. 2 utenti, "PathLog1" "PathLog2".
8.	Per graficare le traiettorie clicca sul tasto play sotto la sezione simulation, seleziona la cartella del log sopra, scegli un intervallo di tempo 
	cioè ad esempio da mezzanotte alle due di notte, e poi il tipo di traiettoria che hai simulato in precedenza (se continua o discreta).
9.	La sezione del designer "Prediction" serve per graficare le traiettorie predette con l'algoritmo con il filtro di Kalman.

Per ulteriori dettagli sulla documentazione, vedi il capitolo 3 della tesi "Design and realization of the simulator".

