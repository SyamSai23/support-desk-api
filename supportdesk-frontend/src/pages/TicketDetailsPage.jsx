import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import api from "../api/axios";
import { useAuth } from "../context/AuthContext";

export default function TicketDetailsPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { role } = useAuth();

  const [ticket, setTicket] = useState(null);
  const [comments, setComments] = useState([]);
  const [agents, setAgents] = useState([]);
  const [message, setMessage] = useState("");
  const [editForm, setEditForm] = useState({
    title: "",
    description: "",
    status: "Open",
  });
  const [assignedToUserId, setAssignedToUserId] = useState("");
  const [error, setError] = useState("");

  const canAssign = role === "Agent" || role === "Admin";
  const canChangeStatus = role === "Agent" || role === "Admin";

  const fetchTicket = async () => {
    try {
      const res = await api.get(`/tickets/${id}`);
      setTicket(res.data);
      setEditForm({
        title: res.data.title,
        description: res.data.description,
        status: res.data.status,
      });
      setAssignedToUserId(res.data.assignedToUserId || "");
    } catch {
      setError("Failed to load ticket");
    }
  };

  const fetchComments = async () => {
    try {
      const res = await api.get(`/tickets/${id}/comments`);
      setComments(res.data || []);
    } catch {
      setComments([]);
    }
  };

  const fetchAgents = async () => {
    if (!canAssign) return;
    try {
      const res = await api.get("/tickets/assignable-agents");
      setAgents(res.data || []);
    } catch {
      setAgents([]);
    }
  };

  const addComment = async (e) => {
    e.preventDefault();
    if (!message.trim()) return;

    try {
      await api.post(`/tickets/${id}/comments`, { message });
      setMessage("");
      fetchComments();
    } catch (err) {
      alert(err.response?.data?.detail || "Failed to add comment");
    }
  };

  const updateTicket = async (e) => {
    e.preventDefault();

    try {
      await api.put(`/tickets/${id}`, editForm);
      await fetchTicket();
      alert("Ticket updated");
    } catch (err) {
      alert(err.response?.data?.detail || "Failed to update ticket");
    }
  };

  const assignTicket = async () => {
    if (!assignedToUserId) return;

    try {
      await api.put(`/tickets/${id}/assign`, {
        assignedToUserId: Number(assignedToUserId),
      });
      await fetchTicket();
      alert("Ticket assigned");
    } catch (err) {
      alert(err.response?.data?.detail || "Failed to assign ticket");
    }
  };

  const deleteTicket = async () => {
    const ok = window.confirm("Are you sure you want to delete this ticket?");
    if (!ok) return;

    try {
      await api.delete(`/tickets/${id}`);
      navigate("/tickets");
    } catch (err) {
      alert(err.response?.data?.detail || "Failed to delete ticket");
    }
  };

  useEffect(() => {
    fetchTicket();
    fetchComments();
    fetchAgents();
  }, [id]);

  if (error) return <div className="max-w-4xl mx-auto mt-8 text-red-500">{error}</div>;
  if (!ticket) return <div className="max-w-4xl mx-auto mt-8">Loading...</div>;

  return (
    <div className="max-w-4xl mx-auto mt-8 bg-white p-6 rounded shadow">
      <h2 className="text-2xl font-bold mb-2">{ticket.title}</h2>
      <p className="mb-3">{ticket.description}</p>
      <p className="text-sm text-gray-600 mb-2">Status: {ticket.status}</p>
      <p className="text-sm text-gray-600 mb-6">
        Assigned To: {ticket.assignedToUserEmail || "Unassigned"}
      </p>

      {canAssign && (
        <div className="border rounded p-4 mb-8">
          <h3 className="text-xl font-semibold mb-4">Assign Ticket</h3>

          <div className="flex gap-3">
            <select
              value={assignedToUserId}
              onChange={(e) => setAssignedToUserId(e.target.value)}
              className="border p-2 rounded flex-1"
            >
              <option value="">Select Agent/Admin</option>
              {agents.map((a) => (
                <option key={a.id} value={a.id}>
                  {a.email}
                </option>
              ))}
            </select>

            <button
              onClick={assignTicket}
              className="bg-purple-600 text-white px-4 py-2 rounded"
            >
              Assign
            </button>
          </div>
        </div>
      )}

      <div className="border rounded p-4 mb-8">
        <h3 className="text-xl font-semibold mb-4">Edit Ticket</h3>

        <form onSubmit={updateTicket} className="flex flex-col gap-4">
          <input
            value={editForm.title}
            onChange={(e) =>
              setEditForm({ ...editForm, title: e.target.value })
            }
            className="border p-2 rounded"
            placeholder="Title"
          />

          <textarea
            value={editForm.description}
            onChange={(e) =>
              setEditForm({ ...editForm, description: e.target.value })
            }
            className="border p-2 rounded"
            rows="4"
            placeholder="Description"
          />

          <select
            value={editForm.status}
            onChange={(e) =>
              setEditForm({ ...editForm, status: e.target.value })
            }
            className="border p-2 rounded"
            disabled={!canChangeStatus}
          >
            <option value="Open">Open</option>
            <option value="InProgress">InProgress</option>
            <option value="Closed">Closed</option>
          </select>

          {!canChangeStatus && (
            <p className="text-sm text-gray-500">
              Only Agent/Admin can change ticket status.
            </p>
          )}

          <div className="flex gap-3">
            <button className="bg-blue-600 text-white px-4 py-2 rounded">
              Save Changes
            </button>

            <button
              type="button"
              onClick={deleteTicket}
              className="bg-red-600 text-white px-4 py-2 rounded"
            >
              Delete Ticket
            </button>
          </div>
        </form>
      </div>

      <h3 className="text-xl font-semibold mb-3">Comments</h3>

      <div className="space-y-3 mb-6">
        {comments.map((c) => (
          <div key={c.id} className="border rounded p-3">
            <p>{c.message}</p>
            <p className="text-sm text-gray-500 mt-1">
              {c.userEmail} • {new Date(c.createdAt).toLocaleString()}
            </p>
          </div>
        ))}
      </div>

      <form onSubmit={addComment} className="flex flex-col gap-3">
        <textarea
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          placeholder="Add comment"
          className="border p-2 rounded"
          rows="4"
        />
        <button className="bg-slate-800 text-white py-2 rounded">
          Post Comment
        </button>
      </form>
    </div>
  );
}