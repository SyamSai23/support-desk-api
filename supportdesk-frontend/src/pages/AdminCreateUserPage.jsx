import { useState } from "react";
import api from "../api/axios";
import { useAuth } from "../context/AuthContext";

export default function AdminCreateUserPage() {
  const { role } = useAuth();
  const [form, setForm] = useState({
    email: "",
    password: "",
    role: "User",
  });
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  if (role !== "Admin") {
    return (
      <div className="max-w-xl mx-auto mt-10 text-red-500">
        Access denied. Admin only.
      </div>
    );
  }

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setMessage("");
    setError("");

    try {
      await api.post("/auth/create-user", form);
      setMessage("User created successfully");
      setForm({ email: "", password: "", role: "User" });
    } catch (err) {
      setError(err.response?.data?.error || "Failed to create user");
    }
  };

  return (
    <div className="max-w-xl mx-auto mt-10 bg-white p-6 rounded shadow">
      <h2 className="text-2xl font-bold mb-4">Admin: Create User</h2>

      {message && <p className="text-green-600 mb-3">{message}</p>}
      {error && <p className="text-red-500 mb-3">{error}</p>}

      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <input
          name="email"
          value={form.email}
          onChange={handleChange}
          placeholder="Email"
          className="border p-2 rounded"
        />

        <input
          name="password"
          type="password"
          value={form.password}
          onChange={handleChange}
          placeholder="Password"
          className="border p-2 rounded"
        />

        <select
          name="role"
          value={form.role}
          onChange={handleChange}
          className="border p-2 rounded"
        >
          <option value="User">User</option>
          <option value="Agent">Agent</option>
          <option value="Admin">Admin</option>
        </select>

        <button className="bg-purple-600 text-white py-2 rounded">
          Create User
        </button>
      </form>
    </div>
  );
}