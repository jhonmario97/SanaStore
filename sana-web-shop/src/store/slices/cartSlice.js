import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import axios from 'axios';



export const addToCart = createAsyncThunk(
    'cart/addToCart',
    async ({ productId, quantity }, { rejectWithValue }) => {
      try {
        const response = await axios.post('https://localhost:7117/api/ShoppingCart/add', null, {
          params: {
            productId,
            quantity,
          },
        });
        return response.data; 
      } catch (error) {
        return rejectWithValue(error.response.data); 
      }
    }
  );

  export const fetchCartItems = createAsyncThunk(
    'cart/fetchCartItems',
    async (_, { rejectWithValue }) => {
      try {
        const response = await axios.get('https://localhost:7117/api/ShoppingCart');
        return response.data;
      } catch (error) {
        return rejectWithValue(error.response.data);
      }
    }
  );
  export const updateCartItemQuantity = createAsyncThunk(
    'cart/updateCartItemQuantity',
    async ({ id, quantity }, { rejectWithValue }) => {
      try {
        const response = await axios.put(
          `https://localhost:7117/api/ShoppingCart/Update`,
          null,
          {
            params: {
              productId: id,
              quantity,
            },
          }
        );
        return response.data; 
      } catch (error) {
        return rejectWithValue(error.response.data); 
      }
    }
  );

  export const SaveOrder = createAsyncThunk(
    'cart/save',
    async ({ orderData }, { rejectWithValue }) => {
      try {
        const response = await axios.post(
          'https://localhost:7117/api/ShoppingCart/save',
          orderData.customerInfo 
        );
        return response.data; 
      } catch (error) {
        return rejectWithValue(error.response.data);
      }
    }
  );
  
  export const deleteCartItem = createAsyncThunk(
    'cart/deleteCartItem',
    async ({ id }, { rejectWithValue }) => {
      try {
        const response = await axios.delete(
          `https://localhost:7117/api/ShoppingCart/delete?productId=`+id,
          null,
         
        );
        return response.data; 
      } catch (error) {
        return rejectWithValue(error.response.data);
      }
    }
  );

const cartSlice = createSlice({
    name: 'cart',
    initialState: {
      items: [],
      status: 'idle',
      error: null,
    },
    reducers: { clearCart: (state) => {
        state.items = [];
      },
 
  },
  extraReducers: (builder) => {
    builder

      .addCase(addToCart.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(addToCart.fulfilled, (state, action) => {
        state.status = 'succeeded';
        const { productId, quantity } = action.meta.arg;
        const existingItem = state.items.find((item) => item.productId === productId);
        if (existingItem) {
          existingItem.quantity += quantity;
        } else {
          state.items.push({ id: productId, quantity });
        }
      })
      .addCase(addToCart.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.payload || 'Failed to add to cart';
      })
      .addCase(fetchCartItems.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(fetchCartItems.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.items = action.payload;
      })
      .addCase(fetchCartItems.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.payload || 'Failed to fetch cart items';
      })
      .addCase(updateCartItemQuantity.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(updateCartItemQuantity.fulfilled, (state, action) => {
        state.status = 'succeeded';
        const { id, quantity } = action.meta.arg;
        const item = state.items.find((item) => item.productId === id);
        if (item) {
          item.quantity = quantity;
        }
      })
      .addCase(updateCartItemQuantity.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.payload || 'Failed to update quantity';
      })
      .addCase(deleteCartItem.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(deleteCartItem.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.items = state.items.filter((item) => item.productId !== action.payload.productId);
      })
      .addCase(deleteCartItem.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.payload || 'Failed to update quantity';
      })
      .addCase(SaveOrder.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(SaveOrder.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.items = [];
      })
      .addCase(SaveOrder.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.payload || 'Failed to update quantity';
      });
  },
});

export const { clearCart  } = cartSlice.actions;
export default cartSlice.reducer;