import { createContext, useContext, useEffect, useState } from "react";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [token, setToken] = useState(localStorage.getItem("token"));
  const [role, setRole] = useState(localStorage.getItem("role"));
  const [email, setEmail] = useState(localStorage.getItem("email"));

  const login = (jwtToken) => {
    localStorage.setItem("token", jwtToken);

    const payload = JSON.parse(atob(jwtToken.split(".")[1]));
    const userRole =
      payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    const userEmail =
      payload["email"] ||
      payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];

    localStorage.setItem("role", userRole || "");
    localStorage.setItem("email", userEmail || "");

    setToken(jwtToken);
    setRole(userRole || "");
    setEmail(userEmail || "");
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    localStorage.removeItem("email");
    setToken(null);
    setRole(null);
    setEmail(null);
  };

  useEffect(() => {
    setToken(localStorage.getItem("token"));
    setRole(localStorage.getItem("role"));
    setEmail(localStorage.getItem("email"));
  }, []);

  return (
    <AuthContext.Provider value={{ token, role, email, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}