<h1>SupportDesk API</h1>

<p>
A lightweight <b>ticket management backend API</b> built using <b>ASP.NET Core</b>.
This project demonstrates a clean RESTful architecture with authentication, DTO-based request handling,
and structured API responses.
</p>

<p>
The system allows users to create, update, and manage support tickets while ensuring secure access
through <b>JWT authentication</b>.
</p>

<hr>

<h2>Tech Stack</h2>

<ul>
<li>C#</li>
<li>ASP.NET Core Web API</li>
<li>Entity Framework Core</li>
<li>SQL Database</li>
<li>JWT Authentication</li>
<li>RESTful API Design</li>
<li>.NET CLI</li>
</ul>

<hr>

<h2>Features</h2>

<ul>
<li>Create support tickets</li>
<li>Update ticket details</li>
<li>Retrieve ticket information</li>
<li>Ticket status management</li>
<li>DTO-based API request handling</li>
<li>JWT authentication and authorization</li>
<li>Clean REST API structure</li>
<li>Database integration</li>
</ul>

<hr>

<h2>Project Structure</h2>

<pre>
SupportDesk.Api
│
├── Controllers
│     TicketController.cs
│
├── Dtos
│     CreateTicketRequest.cs
│     UpdateTicketRequest.cs
│     TicketResponse.cs
│
├── Models
│     Ticket.cs
│
├── Services
│     TicketService.cs
│
├── Data
│     ApplicationDbContext.cs
│
├── Program.cs
└── appsettings.json
</pre>

<hr>

<h2>API Endpoints</h2>

<h3>Create Ticket</h3>

<p><b>POST</b></p>

<pre>/api/tickets</pre>

<p><b>Request Body</b></p>

<pre>
{
  "title": "Login Issue",
  "description": "Unable to login to account"
}
</pre>

<p><b>Response</b></p>

<pre>
{
  "id": 1,
  "title": "Login Issue",
  "description": "Unable to login to account",
  "status": "Open",
  "createdAt": "2026-03-01T12:00:00"
}
</pre>

<hr>

<h3>Get All Tickets</h3>

<p><b>GET</b></p>

<pre>/api/tickets</pre>

<hr>

<h3>Update Ticket</h3>

<p><b>PUT</b></p>

<pre>/api/tickets/{id}</pre>

<p><b>Request</b></p>

<pre>
{
  "title": "Login Issue Updated",
  "description": "Still unable to login",
  "status": "Open"
}
</pre>

<hr>

<h2>Authentication</h2>

<p>This API uses <b>JWT based authentication</b>.</p>

<p>Authenticated requests must include the header:</p>

<pre>
Authorization: Bearer &lt;JWT_TOKEN&gt;
</pre>

<hr>

<h2>Running the Project</h2>

<p><b>Clone the repository</b></p>

<pre>
git clone https://github.com/SyamSai23/support-desk-api.git
</pre>

<p><b>Navigate to the project</b></p>

<pre>
cd support-desk-api
</pre>

<p><b>Run the application</b></p>

<pre>
dotnet run
</pre>

<p>The API will start on:</p>

<pre>
http://localhost:5000
</pre>

<p>or</p>

<pre>
https://localhost:5001
</pre>

<hr>

<h2>Example Workflow</h2>

<ol>
<li>Authenticate using login endpoint</li>
<li>Receive JWT token</li>
<li>Use token for authenticated API requests</li>
<li>Create or manage tickets</li>
</ol>

<hr>

<h2>Future Improvements</h2>

<ul>
<li>Role based authorization</li>
<li>Email notifications</li>
<li>Ticket comments</li>
<li>File attachments</li>
<li>Priority based ticket handling</li>
<li>Admin dashboard</li>
<li>Docker container support</li>
</ul>

<hr>

<h2>Author</h2>

<p>
<b>Syam Sai</b><br>
Software Engineer | Backend Development
</p>

<p>
GitHub:<br>
<a href="https://github.com/SyamSai23">https://github.com/SyamSai23</a>
</p>

<hr>

<h2>License</h2>

<p>
This project is open source and available under the MIT License.
</p>
