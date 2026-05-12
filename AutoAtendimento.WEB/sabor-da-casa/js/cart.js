/**
 * Sabor da Casa - Carrinho
 * cart.js - Gerenciamento do carrinho com LocalStorage
 */

const STORAGE_KEY = "ifood-cart";

const Cart = {
  _items: [],

  load() {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      this._items = raw ? JSON.parse(raw) : [];
    } catch {
      this._items = [];
    }
  },

  save() {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(this._items));
  },

  getItems() {
    return this._items;
  },

  addItem(item) {
    this._items.push(item);
    this.save();
  },

  updateQuantity(id, delta) {
    this._items = this._items
      .map(i => i.id === id ? { ...i, quantity: Math.max(0, i.quantity + delta) } : i)
      .filter(i => i.quantity > 0);
    this.save();
  },

  removeItem(id) {
    this._items = this._items.filter(i => i.id !== id);
    this.save();
  },

  clear() {
    this._items = [];
    this.save();
  },

  getTotal() {
    return this._items.reduce((sum, item) => {
      const extrasTotal = item.extras.reduce((s, e) => s + e.price * e.quantity, 0);
      return sum + (item.sizePrice + extrasTotal) * item.quantity;
    }, 0);
  },

  getCount() {
    return this._items.reduce((sum, item) => sum + item.quantity, 0);
  }
};

// Carregar ao iniciar
Cart.load();
