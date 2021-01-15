# Super Juego de Navecitas (PreALPHA) Unity 2020.1.17f1
![Alt Text](/SJDNCaos1Compressed.gif)

Para el desarrollo de esta práctica se ha propuesto implementar un _bullet hell_ empleando el nuevo sistema DOTS de Unity. Este sistema permite 
mejorar ampliamente el rendimiento de los juegos al estructurar los datos en memoria de forma que se aproveche al máximo la chache así como los diferentes hilos del procesador.

Un gameplay rudimentario se puede encontrar en el siguiente enlace: https://youtu.be/sNvQRO9VpuI 

# TUTORIAL y GAMEPLAY
    El objetivo del juego es sobrevivir a tantas oleadas como sea posible, el juego dispone de una dificultad "dinámica" esto es: Cada vez que el jugador sea capaz de eliminar a todos los enemigos antes de que llegue la siguiente oleada, aumentará la frecuencia de aparición así como el número de enemigos.

    Hay una falta de feedback cuando el enemigo recibe un impacto. En este momento se disminuye su vida y los enemigos cercanos explotan, esto se hace así para que el jugador tenga algo de margen de maniobra. Esto se solventará en futuras versiones añadiendo unos sistemas de partículas.

    Los controles son:
    - LJoystick (mover personaje)
    - RJoystick (apuntar personaje)
    - RTrigger (Disparar)
        



## información general sobre DOTS
El sistema trabaja de la siguiente forma, los GameObjects pasan a llamarse entidades (Entity), estos no tienen funcionalidad solo sirven como identificador, a ellos les podemos añadir componentes (Components) que definen características de la entidad. Ejemplo de esto es por ejemplo un componente que indique la vida del jugador, o un componente que actue como etiqueta para identificarlo. Finalmente los sistemas (Systems) son los encargados de dar comportamiento actuando sobre las entidades. Este comportamiento tiene la ventaja de que al procesar todas las operaciones de un mismo tipo se aumenta tanto la localidad espacial, como temporal de la cache.

## Respecto al desarrollo

La información que presenta Unity es prácticamente nula, se encuentra dispersa y desactualizada de forma generalizada. Es sencillo encontra tipos de sistema que han dejado de existir o de los que la única información disponible es el nombre de la función y los atributos que se le pasan. Además dado que está en _Preview_ resulta sencillo que salten errores sin explicación aparente que se solventan reiniciando el juego o el editor. Finalmente a la hora de depurar el juego resulta muy complejo ya que los mensajes de error aluden a liberías internas del sistema lo cual ayuda poco. 

Algo importante es la pérdida de rendimiento al jugar en el editor, ya que Unity tiene que gestionar toda la información de las entidades para que las puedes visualizar. Se recomienda jugar haciendo una _build_ del juego. (**NOTA** Para ejecutar la _build_ hay que hacerlo como admin)

Hay que hacer notar que aspectos que parecerían sencillos como interactuar con la UI o instanciar partículas, requieren sincronizar los sistemas ya que estos se ejecutan de forma concurrente.

Terminada la ronda de quejas, describimos a continuación los módulos que se han desarrollado.

- Extensión de Cinemachine que permite un mejor control del zoom de la cámara para mantener el encuadre
- Sistema de menús con opciones gráficas
- Modos de 1 y 2 jugadores con soporte para _gamepad_ con el nuevo _Input System_
- _Gameplay_ básico en el que los jugadores han de sobrevivir oleadas y acumular todos los puntos posibles
- Sistema de _Flocking_ que gestiona el movimiento de los enemigos mediante comportamientos emergentes
- Estética _Neon_ empleando la URP y mapas de emisión

Cosas que se han quedado por el camino:

- Sistema de sonido
- Sistema de particulas mediante Pooling
- Sistema de puntuaciones online ***1**
- Tutorial _ingame_
- Mejoras en los patrones de spawn y de los enemigos
- Mejoras de la UI 


Finalmente, respecto al código, se ha comentado todo el proyecto de forma exhaustiva con el fin de que quede claro que hace cada módulo. No obstante, para cualquier duda pongase en contacto conmigo.

***1** Respecto al sistema de puntuación online no se ha podido integrar en el proyecto por falta de tiempo. No obstante sí se ha desarrollado en el lado del servidor. A través de esta url https://superjuegodenavecitasapi.azurewebsites.net/score/1 se pueden hacer llamadas a una api REST desarrollada en python empleando flask. 
