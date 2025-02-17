import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import Catalog from './pages/Catalog';
import ShoppingCart from './pages/ShoppingCart';
import { FaShoppingCart } from "react-icons/fa";
import { GrCatalog } from "react-icons/gr";
function App() {
  return (
    <Router>
      <nav>
        <Link to="/">Catalog <GrCatalog /></Link>
        <Link to="/cart">Shopping Cart <FaShoppingCart /></Link>
      </nav>
      <Routes>
        <Route path="/" element={<Catalog />} />
        <Route path="/cart" element={<ShoppingCart />} />
      </Routes>
    </Router>
  );
}

export default App;