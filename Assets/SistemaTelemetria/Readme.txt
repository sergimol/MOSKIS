Instrucciones del sistema de creación y visualización en tiempo real de curvas.
Añadir el prefab Tracker situado en la carpeta SistemaTelemetria/Prefabs a la primera escena del juego.
Para cada evento sobre el que se quiera hacer seguimiento,crear un script con el mismo formato que los scripts InicioEvent y FinEvent situados en la carpeta SistemaTelemetria/Scripts/Eventos.
También añadir una entrada en el array Event Config del componente Tracker Config del prefab Tracker, en el campo de texto introducir el mismo texto que se haya introducido en la constructora del script del evento.
Configurar las curvas en el componente Graph Persistence del Prefab Tracker.
Si se hace un cambio de escena, añadir el script CanvasCameraSetter a la cámara de la nueva escena.
