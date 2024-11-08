# Simulador de Gestión de Memoria en un Sistema Operativo
En este repositorio se encuentran el código del simulador de gestión de memoria desarrollado por el **Grupo #3** para la clase de **Sistemas Operativos II**.
El simulador permite la asignación y liberación de procesos en memoria usando dos esquemas: **particionamiento fijo** y **particionamiento dinámico**. 

## Requisitos
- .NET Core SDK
- Consola compatible con Unicode (para visualizar los caracteres `█` y `░`).


## Ejecución del Simulador en Visual Studio
1. **Clonar el repositorio** o descargar como archivo ZIP y extraer en la computadora.
2. Abrir el proyecto en **Visual Studio** seleccionando el archivo `.sln` incluido.
3. Ejecutar el proyecto presionando **F5** o seleccionando **Iniciar** en Visual Studio.

## Descripción de Funcionalidades
El simulador de memoria implementa la gestión de procesos mediante dos métodos de particionamiento:

### Particionamiento Fijo
**Descripción**: Divide la memoria en bloques de tamaño fijo. Cada bloque puede contener solo un proceso, sin importar su tamaño real.

### Particionamiento Dinámico
**Descripción**: Asigna a cada proceso un bloque del tamaño exacto que necesita. Esto permite utilizar la memoria de forma más eficiente.


### Visualización del Estado de la Memoria
El simulador muestra el estado de la memoria en forma de barra visual en la consola:
- `█` representa un bloque de memoria ocupado.
- `░` representa un bloque de memoria libre.
Esto permite visualizar fácilmente cuánto espacio está en uso y cuánto queda libre.


### Descripción de Clases Principales

- **Program.cs**: Contiene el punto de entrada `Main` y gestiona la interacción del usuario. Incluye menús y visualización del estado de la memoria.
- **Memoria.cs**: Define una clase abstracta para gestionar la memoria. Contiene métodos abstractos para asignar y liberar procesos.
- **ParticionamientoFijo.cs**: Extiende `Memoria` y maneja la memoria en bloques de tamaño fijo.
- **ParticionamientoDinamico.cs**: Extiende `Memoria` y maneja la memoria en bloques de tamaño variable.
- **Proceso.cs**: Representa cada proceso, con propiedades como ID y tamaño.


## Ejemplos de Uso

1. **Configuración Inicial**:
   - Al iniciar el simulador, se solicita el tamaño total de la memoria y el tipo de particionamiento.
   - Ejemplo: Tamaño de la memoria = `200`, tipo de particionamiento = `Fijo`.

2. **Agregar un Proceso**:
   - Seleccionar la opción para agregar un proceso e ingresar el ID y tamaño del proceso.
   - Ejemplo: ID = `1`, Tamaño = `50`.
   - El simulador mostrará un mensaje indicando si el proceso fue agregado correctamente o si no hay espacio disponible.

3. **Liberar un Proceso**:
   - Seleccionar la opción para liberar un proceso, ingresar el ID del proceso que desea liberar.
   - Ejemplo: ID = `1`.
   - El simulador libera el espacio asignado a ese proceso.

4. **Visualizar el Estado de la Memoria**:
   - Usar la opción para visualizar el estado actual de la memoria, con bloques `█` y `░` para mostrar el espacio ocupado y libre.

## Próximos Avances

En los siguientes entregables, se implementarán:
- **Algoritmos de reemplazo de páginas**: Incluyendo FIFO, LRU y otros.
- **Memoria Virtual**: Permitir que los procesos utilicen memoria más allá de la física disponible.
- **Entre otros requerimientos.**
---
