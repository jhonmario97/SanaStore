import React, { useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { updateCartItemQuantity,clearCart,SaveOrder, deleteCartItem,fetchCartItems } from '../store/slices/cartSlice';
import CustomerInfoModal from '../components/CustomerInfoModal';

const ShoppingCart = () => {
  const dispatch = useDispatch();
  const cartItems = useSelector((state) => state.cart.items);
  const status = useSelector((state) => state.cart.status);
  const error = useSelector((state) => state.cart.error);
  const [isModalOpen, setIsModalOpen] = useState(false);

  useEffect(() => {
    dispatch(fetchCartItems()); 
  }, [dispatch]);


  const handleUpdateQuantity = (id, quantity) => {
    dispatch(updateCartItemQuantity({ id, quantity }))
      .unwrap()
      .then(() => {
        //alert('Quantity updated successfully!');
        

      })
      .catch((error) => {
        alert(`Failed to update quantity: ${error}`);
      });
  };

  const handleRemoveFromCart = (id) => {
    dispatch(deleteCartItem({ id }))
    .unwrap()
    .then(() => {
    //  alert('Item removed successfully!');
      

    })
    .catch((error) => {
      alert(`Failed to remove Item: ${error}`);
    });
  };

  const handleProcessOrder = (customerInfo) => {
    const orderData = {
      customerInfo
    };

    dispatch(SaveOrder({ orderData }))
      .unwrap()
      .then(() => {
        alert('Order saved successfully!');
        dispatch(clearCart()); 
      })
      .catch((error) => {
        alert(`Failed to save order: ${error}`);
      })
      .finally(() => {
        setIsModalOpen(false);
      });
  };
  const totalPrice = cartItems.reduce((total, item) => total + item.price * item.quantity, 0);

  if (status === 'loading') {
    return <div className="loading">Loading...</div>;
  }

  if (status === 'failed') {
    return <div className="error">Error: {error}</div>;
  }

  return (
    <div className="cart">
      <h1>Shopping Cart</h1>
      {cartItems.map((item) => (
        <div className="cart-item" key={item.productId}>
          <h3>{item.title}</h3>
          <p>Item ID: {item.productId}</p>
          <p>Quantity: {item.quantity}</p>
          <p>Price: ${item.price}</p>
          <input
            type="number"
            min="1"
            value={item.quantity}
            onChange={(e) => handleUpdateQuantity(item.productId, parseInt(e.target.value))}
          />
          <button onClick={() => handleRemoveFromCart(item.productId)}>Remove</button>
        </div>
      ))}
      <div className="cart-total">
        <h2>Total: ${totalPrice.toFixed(2)}</h2>
        <button  onClick={() => setIsModalOpen(true)}>Process Order</button>
      </div>
      <CustomerInfoModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSave={handleProcessOrder}
      />
    </div>
  );
};


export default ShoppingCart;