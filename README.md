# Super Juego de Navecitas (PreALPHA) Unity 2020.1.17f1
  **NO HAY SOPORTE PARA RATON Y TECLADO** Se necesitan 1 o 2 mandos para jugar.

  **NOTA** Dado todo el trabajo que tiene que hacer el editor para actualizar el inspector de entidades, el rendimiento en el editor se ve muy mermado. Para probarlo se aconseja usar la build provista.

  **NOTA** Aunque pase la fecha de entrega se espera acabar desarrollando todos los puntos restantes y portarlo a móviles.

![Alt Text](/SJDNCaos1Compressed.gif)

Para el desarrollo de esta práctica se ha propuesto implementar un _bullet hell_ empleando el nuevo sistema DOTS de Unity. Este sistema permite 
mejorar ampliamente el rendimiento de los juegos al estructurar los datos en memoria de forma que se aproveche al máximo la chache así como los diferentes hilos del procesador.

El primer gameplay se puede encontrar en el siguiente enlace: https://youtu.be/sNvQRO9VpuI 
El gameplay actual se puede encontrar en el siguiente enlace: https://youtu.be/rWaNoEykUXo
Una build del juego se puede descargar desde el siguiente enlace: https://drive.google.com/file/d/1KFHkIvxVZJhI-wAoKeBODjS1Fx0h8ELR/view?usp=sharing

# TUTORIAL y GAMEPLAY 
    El objetivo del juego es sobrevivir a tantas oleadas como sea posible, el juego dispone de una dificultad "dinámica" esto es: Cada vez que el jugador sea capaz de eliminar a todos los enemigos antes de que llegue la siguiente oleada, aumentará la frecuencia de aparición así como el número de enemigos.

    Los controles son:
    - LJoystick (mover personaje)
    - RJoystick (apuntar personaje)
    - RTrigger (Disparar)


  
        



## información general sobre DOTS
El sistema trabaja de la siguiente forma, los GameObjects pasan a llamarse entidades (Entity), estos no tienen funcionalidad solo sirven como identificador, a ellos les podemos añadir componentes (Components) que definen características de la entidad. Ejemplo de esto es por ejemplo un componente que indique la vida del jugador, o un componente que actue como etiqueta para identificarlo. Finalmente los sistemas (Systems) son los encargados de dar comportamiento actuando sobre las entidades. Este comportamiento tiene la ventaja de que al procesar todas las operaciones de un mismo tipo se aumenta tanto la localidad espacial, como temporal de la cache.

## Respecto al desarrollo

La información que presenta Unity es prácticamente nula, se encuentra dispersa y desactualizada de forma generalizada. Es sencillo encontra tipos de sistema que han dejado de existir o de los que la única información disponible es el nombre de la función y los atributos que se le pasan. Además dado que está en _Preview_ resulta sencillo que salten errores sin explicación aparente que se solventan reiniciando el juego o el editor. Finalmente a la hora de depurar el juego resulta muy complejo ya que los mensajes de error aluden a liberías internas del sistema lo cual ayuda poco. 

Algo importante es la pérdida de rendimiento al jugar en el editor, ya que Unity tiene que gestionar toda la información de las entidades para que las puedes visualizar. Se recomienda jugar haciendo una _build_ del juego. (**NOTA** Para ejecutar la _build_ (en algunos casos) hay que hacerlo como admin)

Hay que hacer notar que aspectos que parecerían sencillos como interactuar con la UI o instanciar partículas, requieren sincronizar los sistemas ya que estos se ejecutan de forma concurrente.

Describimos a continuación los módulos que se han desarrollado.

- Extensión de Cinemachine que permite un mejor control del zoom de la cámara para mantener el encuadre
- Sistema de menús con opciones gráficas
- Modos de 1 y 2 jugadores con soporte para _gamepad_ con el nuevo _Input System_
- _Gameplay_ básico en el que los jugadores han de sobrevivir oleadas y acumular todos los puntos posibles
- Sistema de _Flocking_ que gestiona el movimiento de los enemigos mediante comportamientos emergentes
- Estética _Neon_ empleando la URP y mapas de emisión
- Sistema de sonido
- Sistema de particulas mediante Pooling
- Mejoras de la UI
- Tutorial 

Cosas que se han quedado por el camino:

- Sistema de puntuaciones online ***1**
- Mejoras en los patrones de spawn y de los enemigos
- Animación fin de partida
- Más mejoras de la UI 


Finalmente, respecto al código, se ha comentado todo el proyecto de forma exhaustiva con el fin de que quede claro que hace cada módulo. No obstante, para cualquier duda pongase en contacto conmigo.

***1** Respecto al sistema de puntuación online no se ha podido integrar en el proyecto por falta de tiempo. No obstante sí se ha desarrollado en el lado del servidor. A través de esta url https://superjuegodenavecitasapi.azurewebsites.net/score/1 se pueden hacer llamadas a una api REST desarrollada en python empleando flask.

# Detalles de implementacion

A grandes rasgos, el "loop" del juego es el siguiente, entrecomillas porque todo es concurrente y asíncrono. El jugador mediante el mando produce un input, este input es capturado por InputSystem.cs y prepara los datos para que sean procesados por PlayerMovementSystem.cs. Los enemigos son desplegados por Spawner.cs, según se instancian los enemigos, el sistema EnemyTargettingSystem.cs los recoge y hace que se muevan en dirección al jugador. En caso de que el jugador les acierte con un disparo (HitSystem.cs) los enemigos son marcados como destruidos, y otro sistema se encarga de borrarlos instanciando en su posición un sistema de partículas empleando la clase PoolingScript para hacerlo de forma eficiente.

Respecto a la UI, y teniendo en cuenta mi inexperiencia con ella, la resolución se fija a FullHD en caso que sea posible, si no se adapta a la resolución nativa del monitor. Lo que puede dar lugar en algunos casos a textos desplazados, pero no debería influir en la jugabilidad.

A continuación describiremos el funcionamiento de algunas de las clases más importantes. Las clases que describiremos son las siguientes:

- HitSystem.cs 
- DestroyEnemies.cs
- EnemyTargettingSystem.cs
- InputSystem.cs
- VoidsSystem.cs
- PlayerMovementSystem.cs
- PoolingScript.cs
- SceneManager.cs
- Spawner.cs

## HitSystem.cs

Este sistema se encarga de gestionar las colisiones entre las balas y los enemigos así como de la interacción del jugador y el enemigo. Encontramos dos Jobs principales EnemyOnHitTriggerJob y BulletOnHitTriggerJob. El primero se encarga de detecar si un jugador ha chocado contra el enemigo, en ese caso se marca al jugador como dañado. En el segundo se opera de forma similar, cuando un proyectil alcanza a un enemigo lo marca como dañado.

## DestroyEnemies.cs

En este sistema iteramos sobre los enemigos, en caso de que hayan recibido daños, lo destruimos e instanciamos una explosión empleando PoolingScript.cs. También se comprueba si un jugador ha recibido daños, destruyendo los enemigos colindantes para dar algo de margen de maniobra.

## EnemyTargettingSystem.cs

Este sistema se encarga de dirigir a los enemigos hacia el jugador más cercano para ello emplea _steering behaviours_ lo que hace posible esquivarlos.

## InputSystem.cs 

InputySystem.cs se encarga de captar el input del mando, y escribirlo en el componente que lleva asociado el jugador, este sistema permite detectar en que escena estamos asociando a cada jugador un mando distinto.

## VoidsSystem.cs

Este sistema se encarga de gestionar los _voids_ de las pantallas de inicio y fin, calcula los voids cercanos y hace que se alineen manteniendo cierta separación los unos con los otros.


## PlayerMovementSystem.cs

Es el encargado de una vez se han recogido los inputs por el InputSystem.cs, procesarlos asignando una translación y una rotación al jugador.

## PoolingScript.cs

PoolingScript permite instanciar diferentes sistemas de partículas de forma eficiente, internamente se cuenta con tantas colas como tipos de sistema de partículas tengamos. Cuando se decide instanciar uno, se coge el primer elemento de la cola y se pone al final. De esta forma evitar crear nuevos efectos cada vez que se requieren. El usario puede determinar cuandos sistemas del mismo típo pueden existir simultáneamente.  

# SceneManager.cs

Es el encargado de actualizar la puntuación y la salud restante del jugador. En caso de que la salud llegue a 0 destruimos todas las entidades y pasamos a la pantalla de fin de juego.

# Spawner.cs

Finalmente la clase Spawner instancia las oleadas de enemigos. A medida que avanza el tiempo, se instancia un número determinado de enemigos formando un círculo alrededor de los jugadores. La frecuencia de instanciado así como el número dependen de la eficacia de los jugadores. Si los jugadores consiguen destruir a todos los enemigos antes de que empiece la siguiente ronda, se instanciarán más y más rápido.
