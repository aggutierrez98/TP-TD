# Asistente financiero

_Especificaciones de casos de uso_

---

**ID y Nombre:** CUN87 Conversar y consultar recomendación financiera  
**Estado:** Implementado  
**Descripcion:** Permite al Usuario, Operador o Administrador conversar con el asistente personal y obtener respuestas y recomendaciones sobre sus finanzas, como dónde comprar los productos de su lista, qué hacer con su saldo, presupuesto y ahorro.  
**Actor Principal:** Usuario, Operador o Administrador  
**Actor Secundario:** Servicio externo de inteligencia artificial (Gemini) y fuentes externas de datos de mercado  
**Precondiciones:** El actor tiene una sesión iniciada y cuenta con autorización para usar el asistente personal. Para recibir recomendaciones de compra por cercanía, el actor registró sus zonas y tiene productos pendientes en su lista de compras. Para recibir recomendaciones personales de inversión, el actor completó su perfil inversor.  
**Puntos de Extension:** Las recomendaciones de compra usan las zonas registradas en CUN79 Alta de zona de usuario. Las recomendaciones de inversión requieren el perfil de CUN77 Completar o actualizar mi perfil inversor.  
**Condicion:** El actor desea consultar una duda o pedir una recomendación sobre sus finanzas personales.  

**Escenario Principal:**
1. El actor selecciona la opción «Chatear con el asistente» del menú «Asistente Personal».
2. El sistema presenta la pantalla «Asistente Personal» con el historial de la conversación (cada mensaje precedido por la hora y por «Vos» o «Asistente»), el campo «Tu mensaje:» y el botón «Enviar».
3. El actor escribe su consulta en el campo «Tu mensaje:» y selecciona el botón «Enviar» (o presiona Enter).
4. El sistema deshabilita el campo y el botón, muestra el cursor de espera y consulta al servicio externo de inteligencia artificial y, cuando la consulta lo requiere, a las fuentes de datos de mercado.
5. El sistema agrega al historial la consulta del actor y la respuesta del asistente, con la hora de cada mensaje, desplaza el historial hasta el último mensaje, limpia el campo «Tu mensaje:», vuelve a habilitar los controles y coloca el foco en el campo para continuar la conversación.

**Flujos Alternativos:**
2a. Si el actor ya conversó antes con el asistente, el sistema presenta en el historial los intercambios anteriores, del más antiguo al más reciente. Los rechazos mostrados en conversaciones previas no se conservan.
2b. Si ocurre un error al cargar el historial, el sistema muestra un mensaje de error y presenta el historial vacío.
3a. Si el actor desea escribir varias líneas, presiona Shift+Enter para insertar un salto de línea sin enviar el mensaje.
3b. Si el mensaje está vacío o contiene solo espacios, el sistema muestra el mensaje «Debe escribir un mensaje.» y no envía la consulta.
3c. Si el actor intenta enviar otra consulta mientras hay una en curso, el sistema la ignora hasta que se reciba la respuesta pendiente.
5a. Si el actor realizó demasiadas consultas seguidas, el asistente responde en el historial «Hiciste muchas preguntas seguidas. Espera un minuto y volve a intentar.».
5b. Si la consulta no trata sobre finanzas personales, es demasiado extensa (más de 500 caracteres), contiene contenido no permitido o no pudo clasificarse, el asistente responde en el historial «Solo puedo ayudarte con tus finanzas personales: compras de tu lista, que hacer con tu saldo, presupuesto y ahorro. Proba reformular la pregunta.».
5c. Si la respuesta generada no supera las validaciones del sistema, el asistente responde en el historial «No pude generar una respuesta confiable para esta pregunta. Proba reformularla.».
5d. Si la consulta es una recomendación de inversión y el actor no completó su perfil inversor, el asistente responde en el historial que primero necesita completar el perfil inversor desde «Mi Configuración» con «Completar cuestionario».
5e. Si la consulta de inversión indica un porcentaje no válido (menor o igual a cero o mayor a 100), el asistente solicita un valor mayor a 0 y hasta 100 %, o una consulta sin porcentaje, e incluye el aviso «Información orientativa, no constituye asesoramiento financiero.».
5f. Si la consulta es una recomendación de compra y el actor no registró zonas, el asistente le indica que las agregue en «Mi usuario > Mis zonas» para recibir recomendaciones por cercanía.
5g. Si la recomendación de compra o de inversión no puede completarse con los datos disponibles, el asistente informa en su respuesta qué productos o fuentes no pudo considerar.
5h. Si falta configurar el servicio de inteligencia artificial, el sistema muestra el mensaje «Falta configurar la clave de API de Gemini.» o «El asistente financiero no está configurado.». La consulta no se agrega al historial.
5i. Si no se pudieron obtener datos válidos de las fuentes de mercado, el sistema muestra el mensaje «No se pudieron obtener datos válidos de la fuente de mercado.». La consulta no se agrega al historial.
5j. Si ocurre otro error al obtener o guardar la respuesta, el sistema muestra el mensaje «No se pudo obtener respuesta del asistente financiero.» o «No se pudo guardar la consulta al asistente financiero.». La consulta no se agrega al historial y el campo «Tu mensaje:» conserva el texto escrito.
5k. Si el actor cierra la pantalla mientras hay una consulta en curso, el sistema descarta la respuesta cuando llega, sin mostrarla.

**Postcondiciones:** La consulta y la respuesta del asistente quedan en el historial de la conversación del actor, disponibles en sus próximas visitas a la pantalla. Los rechazos se muestran en la conversación abierta pero no se conservan. Si la consulta no se completó, el historial permanece sin cambios.
