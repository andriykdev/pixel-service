# pixel-service

Peak Ventures Technical Task - Backend Engineer

### This README provides an overview of the approach taken to address the technical task.

Given the constraints and requirements outlined in the technical task, a message queue architecture was chosen to handle communication between Pixel and Storage services.
This approach offers benefits such as decoupling of services, scalability, and fault tolerance.
It also accommodates scenarios where services may run in different networks.

**Advantages**: independent scaling and fault isolation, can handle bursts in traffic and ensures data integrity even if one of the services is temporarily unavailable.

**Downsides and Mitigation**: while the message queue approach offers numerous advantages, it does introduce complexities such as message broker setup and potential message loss.

Alternatives like `HTTP via RESTful API`, `gRPC`, or `WebSockets` were considered but deemed not superior in this context.

**To mitigate potential downsides**:
- Messages are consumed one by one and appended to the file sequentially.
- Acknowledgement of delivery ensures successful message saving.
- In case the Storage service is down, messages are saved in the queue until processing resumes.

`RabbitMQ` was chosen as an example message broker, but alternatives like `SQS`, `Azure Service Bus`, or `Kafka` can be utilized based on preferences or requirements.
The `Storage Service` is implemented as a worker service, responsible for consuming messages from the queue and writing them to the file.

Throughout the implementation, efforts were made to maintain simplicity while meeting the task requirements.
