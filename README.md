Steps to run the application locally:

- Start Docker Desktop and run Redis with this following commands : 
docker pull redis:latest
docker run -d --name my-redis -p 6379:6379 redis:latest
- Backend: Open the .NET Core 8 solution and run it. The Swagger UI will automatically be available
- Frontend: Navigate to the React.js project folder, run npm install to install dependencies, then npm start to run the frontend.
- Login: You can log in using the default credentials:
Username: admin
Password: admin123
Alternatively, you can create a new user via the Swagger UI.