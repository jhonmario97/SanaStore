import React, { useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchProducts } from '../store/slices/productSlice';
import { addToCart } from '../store/slices/cartSlice';

const Catalog = () => {
  const dispatch = useDispatch();
  const products = useSelector((state) => state.products.items);
  const status = useSelector((state) => state.products.status);
  const error = useSelector((state) => state.products.error);
  const [quantities, setQuantities] = useState({});
  const [currentPage, setCurrentPage] = useState(1);
  const productsPerPage = 10;

  useEffect(() => {
    if (status === 'idle') {
      dispatch(fetchProducts());
    }
  }, [status, dispatch]);

  const handleAddToCart = (product) => {
    const quantity = quantities[product.id] || 1;
    if (quantity > product.stock) {
      alert('Not enough stock available');
      return;
    }
    dispatch(addToCart({ productId: product.productId, quantity }))
      .unwrap()
      .then(() => {
        alert('Product added to cart successfully!');
      })
      .catch((error) => {
        alert(`Failed to add to cart: ${error}`);
      });
  };

  // Pagination logic
  const indexOfLastProduct = currentPage * productsPerPage;
  const indexOfFirstProduct = indexOfLastProduct - productsPerPage;
  const currentProducts = products.slice(indexOfFirstProduct, indexOfLastProduct);

  const paginate = (pageNumber) => setCurrentPage(pageNumber);

  if (status === 'loading') {
    return <div className="loading">Loading...</div>;
  }

  if (status === 'failed') {
    return <div className="error">Error: {error}</div>;
  }

  return (
    <div className="catalog" >
 
      {currentProducts.map((product) => (
        <div className="product-card"  key={product.id}>
          <h2>{product.title}</h2>
          <p>{product.description}</p>
          <div class="image-container">
            <img 
            src={`data:image/png;base64,${product.image}`} 
            alt="Base64 Image" />
          </div>
          <p>Price: ${product.price}</p>
          <p>Stock: {product.stock}</p>
          <input
            type="number"
            min="1"
            defaultValue={1}
            max={product.stock}
           
            onChange={(e) =>
              setQuantities({ ...quantities, [product.id]: parseInt(e.target.value) })
            }
          />
          <button onClick={() => handleAddToCart(product)}>Add to Cart</button>
        </div>
      ))}
      <div className="pagination">
        {Array.from({ length: Math.ceil(products.length / productsPerPage) }, (_, i) => (
          <button key={i + 1} onClick={() => paginate(i + 1)}>
            {i + 1}
          </button>
        ))}
      </div>
    </div>
  );
};

export default Catalog;