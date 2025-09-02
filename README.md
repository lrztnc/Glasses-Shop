# Presentazione dell'applicazione #
## Descrizione generale ##
L'applicazione rappresenta un e-commerce per la vendita di occhiali, sviluppata in Unity. Le funzionalità utilizzabili dagli utenti sono:
- Registrarsi e fare il login
- Sfogliare un catalogo dinamico dei prodotti disponibili
- Cercare e filtrare i prodotti
- Aggiungere gli articoli nel carrello, con il calcolo del totale aggiornato
- Provare virtualmente gli occhiali

## Classi principali ##
**LoginManager.cs:** gestione e autenticazoine.
**UIManager.cs:** controllo dei pannelli UI (Shop, cart, Try-On, Login, ...)
**ShopManager.cs:** gestione del catalogo prodotti, pulsanti Add to Cart, e caricamento dinamico
**CartManager.cs:** gestione del carrello (aggiunta, rimozione totale)
**ProductUI.cs:** script per il prefab dei prodotti
**ProductSearch.cs:** gestione della barra di ricerca e del suo filtraggio
**GlassFollower.cs:** logica per 'agganciare' gli occhiali attraverso Face Detection
**UserData.cs:** salvataggio delle informazione utente

## Prototipo Interfaccia ##
**Login/Signup panel:** campi utente e password
**Shop panel:** catologo (scroll view) con l'aggiunta della search bar e dropdawn.
**Cart panel:** lista dei prodotti scelti, pulsante per la rimozione, caricamento dei dati utente e del totale
**TryOn panel:** webcam con 'l'agganciaento' degli occhiali al viso

## Tecnologie usate ##
- **Unity:** sviluppo della piattaforma
- **JSON:** per il caricamento dinamico dei prodotti
- **MediaPipe:** per il tracking del volto
- **Script C#:** per lo sviluppo della logica del programma

## Difficoltà riscontrate ##
- Aggangio degli occhiali al volto, con l'uso dei prefab immobili
- Ricaricamento del totale: con la rimozione di alcuni prodotti il tolale o restava a '0,00€' oppure il calcolo era sbagliato
- Accensione della webcam