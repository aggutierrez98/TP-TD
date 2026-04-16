# Arquitectura de 7 Capas

Este proyecto implementa rigurosamente el modelo de 7 capas:

1. **UI/PRESENTACION**: Interacción con el usuario de manera directa a través de formularios. Esta capa exclusivamente se comunicará con **BLL** apoyada en entidades de **BE**.
2. **BLL (Business Logic Layer)**: Centraliza validaciones, reglas de negocio y oficia como puente validando datos antes de que interactúen con el almacenamiento o los servicios.
3. **Mappers**: Un mapper por clase de Entidad de Negocio (BE). Encargado de trasladar datos del dominio a operaciones en la **DAL**.
4. **DAL (Data Access Layer)**: Lógica de acceso y persistencia en BD, de forma completamente agnóstica de la tecnología final desde la perspectiva de la BLL.
5. **BE (Business Entities)**: Transversal a todo el sistema. Consta de clases de dominio donde NO se admiten métodos de comportamiento, únicamente el estado (Propiedades).
6. **Servicios (SERVICIOS)**: Transversal; alberga servicios (Autenticación, Hash, Auditoría, Sesiones) consumibles por cualquier componente.
7. **Abstracciones (ABSTRACCIONES)**: Aplica el principio de Inversión de Dependencias. Conserva interfaces asegurándose de que los diferentes módulos no requieran la implementación del otro para compilar.
