# Simulador de Gestión de Memoria en un Sistema Operativo

Este repositorio contiene el código del **Simulador de Gestión de Memoria** desarrollado por el **Grupo #3** para la clase de **Sistemas Operativos II**. El simulador permite la asignación y liberación de procesos en memoria usando los siguientes esquemas: **particionamiento fijo**, **particionamiento dinámico**, **paginación**, **segmentación**, y **memoria virtual**.

---

## Requisitos
- .NET Core SDK
- Consola compatible con Unicode (para visualizar los caracteres `█` y `░`).

---

## Ejecución del Simulador en Visual Studio
1. **Clonar el repositorio** o descargar como archivo ZIP y extraer en la computadora.
2. Abrir el proyecto en **Visual Studio** seleccionando el archivo `.sln` incluido.
3. Ejecutar el proyecto presionando **F5** o seleccionando **Iniciar** en Visual Studio.

---

## Funcionalidades Implementadas

### Gestión de Memoria con Particionamiento
- **Particionamiento Fijo**: 
  - Divide la memoria en bloques de tamaño fijo.
  - Cada bloque puede contener solo un proceso, sin importar su tamaño real.
- **Particionamiento Dinámico**:
  - Asigna a cada proceso un bloque del tamaño exacto que necesita.
  - Permite una mejor utilización de la memoria, pero puede causar fragmentación externa.

### Paginación
- **Esquema de Paginación**:
  - Los procesos se dividen en páginas, mientras que la memoria se divide en marcos.
  - Soporta el uso de **memoria virtual** para extender el espacio disponible más allá de la memoria física.
- **Memoria Virtual**:
  - Los procesos pueden crecer más allá de la memoria física disponible.
  - Implementación de **algoritmos de reemplazo de páginas**, incluyendo:
    - **FIFO**: Reemplaza la página más antigua en memoria.

### Segmentación
- **Esquema de Segmentación**:
  - Divide los procesos en segmentos lógicos como código, datos y pila.
  - Maneja la fragmentación interna y externa de manera explícita.

### Visualización del Estado de la Memoria
- El simulador muestra el estado actual de la memoria en la consola:
  - `█` representa un bloque de memoria ocupado.
  - `░` representa un bloque de memoria libre.
- Muestra claramente las siguientes métricas:
  - **Memoria Física Total, Usada y Libre.**
  - **Memoria Virtual Libre.**
  - Detalles de los procesos en memoria, con indicación de si están en **memoria física** o **memoria virtual**.

### Simulación de Fallos de Página y Fragmentación
- Simula fallos de página cuando no hay suficiente memoria física disponible.
- Manejo de fragmentación interna y externa en particionamiento dinámico y segmentación.
- Los algoritmos de reemplazo y compactación se aplican para optimizar el uso de memoria.

---

## Ejemplos de Uso

1. **Configuración Inicial**:
   - Al iniciar el simulador, se solicita el tamaño total de la memoria, el tipo de particionamiento (fijo, dinámico, paginación o segmentación) y, si corresponde, el tamaño de las páginas o segmentos.
   - Ejemplo: Tamaño de la memoria = `200`, esquema = `Paginación`, tamaño de página = `10`.

2. **Agregar un Proceso**:
   - Seleccionar la opción para agregar un proceso, ingresando el ID y el tamaño del proceso.
   - Ejemplo: ID = `1`, Tamaño = `50`.
   - El simulador asignará el proceso en memoria física o virtual, dependiendo del esquema seleccionado y el espacio disponible.

3. **Liberar un Proceso**:
   - Seleccionar la opción para liberar un proceso, ingresando el ID del proceso que desea liberar.
   - Ejemplo: ID = `1`.
   - El simulador libera el espacio ocupado y actualiza el estado de la memoria.

4. **Visualizar el Estado de la Memoria**:
   - Seleccionar la opción para ver el estado actual de la memoria.
   - Ejemplo: 
     ```
     Memoria Física Total: 100
     Memoria Física Usada: 50
     Memoria Física Libre: 50
     Memoria Virtual Libre: 100
     ```

5. **Simulación de Fallos de Página**:
   - Cuando no hay marcos libres en memoria física, se utiliza un algoritmo de reemplazo para mover páginas a memoria virtual.

---

## Requerimientos del Proyecto
El simulador incluye los siguientes requisitos:

### Gestión de Memoria con Particionamiento Fijo y Dinámico
- **Simulación de particionamiento fijo** con bloques de tamaño fijo.
- **Simulación de particionamiento dinámico**, asignando bloques del tamaño exacto necesario.

### Paginación y Segmentación
- **Esquema de paginación**:
  - Los procesos se dividen en páginas, y la memoria en marcos.
  - Implementación de memoria virtual con soporte para reemplazo de páginas.
- **Esquema de segmentación**:
  - Los procesos se dividen en segmentos lógicos (código, datos, pila).

### Memoria Virtual
- Permite que los procesos crezcan más allá de la memoria física disponible.
- Simulación de memoria virtual con manejo de fallos de página.

### Algoritmos de Reemplazo de Páginas
- Implementación de:
  - **FIFO (First In, First Out)**.
  - **LRU (Least Recently Used)**.
  - **Reloj**.
- Visualización del proceso de reemplazo cuando la memoria está llena.

### Visualización del Estado de la Memoria
- Visualización gráfica del estado actual de la memoria:
  - **Memoria Física**: Bloques ocupados y libres.
  - **Memoria Virtual**: Páginas asignadas y libres.
- Detalles de cada proceso y ubicación de sus páginas/segmentos.

### Simulación de Fallos de Página y Fragmentación
- Simulación de fallos de página en esquemas de paginación y memoria virtual.
- Manejo explícito de fragmentación interna y externa en particionamiento y segmentación.
- Implementación de compactación y reemplazo de páginas para optimizar la memoria.

---

## Próximos Avances
- Integración de **algoritmos adicionales de reemplazo de páginas**.
- Optimización de la segmentación y manejo de fragmentación externa.
- Mejora de la visualización gráfica para incluir más detalles sobre los procesos y su estado.

Este simulador proporciona una herramienta educativa para comprender los conceptos clave de la gestión de memoria en sistemas operativos.
