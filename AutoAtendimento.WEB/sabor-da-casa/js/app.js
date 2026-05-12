/**
 * Sabor da Casa - Cardápio Digital
 * app.js - Lógica principal do cardápio
 */

// ===== Dados do Cardápio =====
const categories = [
  { id: "almoco", name: "Almoço", icon: "🍽️" },
  { id: "petiscos", name: "Petiscos", icon: "🍢" },
  { id: "bebidas", name: "Bebidas", icon: "🥤" },
];

const products = [
  {
    id: "moqueca", name: "Moqueca de Peixe",
    description: "Peixe fresco cozido no leite de coco com dendê, pimentões e coentro",
    image: "assets/images/moqueca.jpg", category: "almoco",
    sizes: [{ name: "Pequena", price: 32.90 }, { name: "Média", price: 45.90 }, { name: "Grande", price: 59.90 }],
    extras: [{ name: "Arroz", price: 5.00 }, { name: "Farofa", price: 4.00 }, { name: "Pirão", price: 6.00 }],
  },
  {
    id: "picanha", name: "Picanha Grelhada",
    description: "Picanha na brasa com farofa, vinagrete e arroz",
    image: "assets/images/picanha.jpg", category: "almoco",
    sizes: [{ name: "Pequena", price: 42.90 }, { name: "Média", price: 55.90 }, { name: "Grande", price: 72.90 }],
    extras: [{ name: "Arroz", price: 5.00 }, { name: "Farofa", price: 4.00 }, { name: "Vinagrete extra", price: 3.00 }],
  },
  {
    id: "feijoada", name: "Feijoada Completa",
    description: "Feijoada com arroz, couve, laranja e farofa crocante",
    image: "assets/images/feijoada.jpg", category: "almoco",
    sizes: [{ name: "Pequena", price: 29.90 }, { name: "Média", price: 39.90 }, { name: "Grande", price: 52.90 }],
    extras: [{ name: "Arroz", price: 5.00 }, { name: "Farofa", price: 4.00 }, { name: "Couve extra", price: 3.50 }],
  },
  {
    id: "frango", name: "Frango Grelhado",
    description: "Peito de frango grelhado com temperos especiais e limão",
    image: "assets/images/frango.jpg", category: "almoco",
    sizes: [{ name: "Pequena", price: 24.90 }, { name: "Média", price: 34.90 }, { name: "Grande", price: 44.90 }],
    extras: [{ name: "Arroz", price: 5.00 }, { name: "Farofa", price: 4.00 }, { name: "Salada", price: 4.50 }],
  },
  {
    id: "coxinha", name: "Coxinha de Frango",
    description: "Coxinhas crocantes recheadas com frango desfiado e catupiry",
    image: "assets/images/coxinha.jpg", category: "petiscos",
    sizes: [{ name: "Pequena", price: 14.90 }, { name: "Média", price: 22.90 }, { name: "Grande", price: 29.90 }],
    extras: [{ name: "Molho especial", price: 3.00 }, { name: "Catupiry extra", price: 4.00 }],
  },
  {
    id: "pastel", name: "Pastel de Carne",
    description: "Pastéis crocantes recheados com carne moída temperada",
    image: "assets/images/pastel.jpg", category: "petiscos",
    sizes: [{ name: "Pequena", price: 12.90 }, { name: "Média", price: 19.90 }, { name: "Grande", price: 26.90 }],
    extras: [{ name: "Molho especial", price: 3.00 }, { name: "Queijo extra", price: 4.00 }],
  },
  {
    id: "bolinho", name: "Bolinho de Bacalhau",
    description: "Bolinhos dourados de bacalhau com batata e salsa",
    image: "assets/images/bolinho-bacalhau.jpg", category: "petiscos",
    sizes: [{ name: "Pequena", price: 18.90 }, { name: "Média", price: 27.90 }, { name: "Grande", price: 34.90 }],
    extras: [{ name: "Molho tártaro", price: 3.50 }, { name: "Limão", price: 1.00 }],
  },
  {
    id: "sucos", name: "Suco Natural",
    description: "Suco de frutas frescas: laranja, maracujá ou limão",
    image: "assets/images/sucos.jpg", category: "bebidas",
    sizes: [{ name: "Pequena", price: 8.90 }, { name: "Média", price: 12.90 }, { name: "Grande", price: 15.90 }],
    extras: [{ name: "Açúcar extra", price: 0.50 }],
  },
  {
    id: "guarana", name: "Guaraná",
    description: "Guaraná Antarctica gelado",
    image: "assets/images/guarana.jpg", category: "bebidas",
    sizes: [{ name: "Pequena", price: 5.90 }, { name: "Média", price: 7.90 }, { name: "Grande", price: 10.90 }],
    extras: [{ name: "Gelo extra", price: 0.50 }],
  },
  {
    id: "caipirinha", name: "Caipirinha",
    description: "Caipirinha clássica de limão com cachaça artesanal",
    image: "assets/images/caipirinha.jpg", category: "bebidas",
    sizes: [{ name: "Pequena", price: 14.90 }, { name: "Média", price: 19.90 }, { name: "Grande", price: 24.90 }],
    extras: [{ name: "Dose extra", price: 5.00 }, { name: "Frutas mix", price: 3.00 }],
  },
];

// ===== Utilidades =====
function formatPrice(value) {
  return "R$ " + value.toFixed(2).replace(".", ",");
}

function generateId() {
  return Math.random().toString(36).substring(2, 10);
}

// ===== Renderizar Cardápio =====
function renderMenu() {
  const menuEl = document.getElementById("menu");
  menuEl.innerHTML = "";

  categories.forEach(cat => {
    const catProducts = products.filter(p => p.category === cat.id);
    const section = document.createElement("div");
    section.id = cat.id;

    section.innerHTML = `
      <h2 class="section-title">${cat.icon} ${cat.name}</h2>
      ${catProducts.map(p => `
        <div class="product-card" onclick="openModal('${p.id}')">
          <img src="${p.image}" alt="${p.name}" class="product-card__img" loading="lazy">
          <div class="product-card__info">
            <div class="product-card__name">${p.name}</div>
            <div class="product-card__desc">${p.description}</div>
            <div class="product-card__price">a partir de ${formatPrice(p.sizes[0].price)}</div>
          </div>
        </div>
      `).join("")}
    `;

    menuEl.appendChild(section);
  });
}

// ===== Category Bar =====
function renderCategoryBar() {
  const bar = document.getElementById("category-bar");
  bar.innerHTML = categories.map(cat =>
    `<button class="category-btn" data-cat="${cat.id}" onclick="scrollToCategory('${cat.id}')">${cat.icon} ${cat.name}</button>`
  ).join("");
}

function scrollToCategory(id) {
  const el = document.getElementById(id);
  if (el) {
    const offset = 60;
    const top = el.getBoundingClientRect().top + window.scrollY - offset;
    window.scrollTo({ top, behavior: "smooth" });
  }
  setActiveCategory(id);
}

function setActiveCategory(id) {
  document.querySelectorAll(".category-btn").forEach(btn => {
    btn.classList.toggle("active", btn.dataset.cat === id);
  });
}

// Intersection Observer para destacar categoria ativa
function setupCategoryObserver() {
  const observer = new IntersectionObserver(entries => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        setActiveCategory(entry.target.id);
      }
    });
  }, { rootMargin: "-80px 0px -70% 0px", threshold: 0 });

  categories.forEach(cat => {
    const el = document.getElementById(cat.id);
    if (el) observer.observe(el);
  });
}

// ===== Modal do Produto =====
let modalState = { selectedSize: 0, extras: {} };

function openModal(productId) {
  const product = products.find(p => p.id === productId);
  if (!product) return;

  modalState = {
    product,
    selectedSize: 0,
    extras: {},
    observation: "",
  };
  product.extras.forEach(e => modalState.extras[e.name] = 0);

  renderModal();
  document.getElementById("modal-overlay").classList.remove("hidden");
  document.body.style.overflow = "hidden";
}

function closeModal() {
  document.getElementById("modal-overlay").classList.add("hidden");
  document.body.style.overflow = "";
}

function selectSize(index) {
  modalState.selectedSize = index;
  renderModal();
}

function updateExtra(name, delta) {
  modalState.extras[name] = Math.max(0, (modalState.extras[name] || 0) + delta);
  renderModal();
}

function getModalTotal() {
  const p = modalState.product;
  const sizePrice = p.sizes[modalState.selectedSize].price;
  const extrasTotal = p.extras.reduce((sum, e) => sum + e.price * (modalState.extras[e.name] || 0), 0);
  return sizePrice + extrasTotal;
}

function renderModal() {
  const p = modalState.product;
  const modal = document.getElementById("modal-content");

  modal.innerHTML = `
    <div class="modal__header">
      <img src="${p.image}" alt="${p.name}" class="modal__img">
      <button class="modal__close" onclick="closeModal()">✕</button>
    </div>
    <div class="modal__body">
      <h2 class="modal__title">${p.name}</h2>
      <p class="modal__desc">${p.description}</p>

      <h3 class="modal__section-title">Tamanho</h3>
      <div class="sizes-row">
        ${p.sizes.map((s, i) => `
          <button class="size-btn ${modalState.selectedSize === i ? "active" : ""}" onclick="selectSize(${i})">
            ${s.name}
            <span class="size-btn__price">${formatPrice(s.price)}</span>
          </button>
        `).join("")}
      </div>

      ${p.extras.length > 0 ? `
        <h3 class="modal__section-title">Extras</h3>
        ${p.extras.map(e => `
          <div class="extra-row">
            <div>
              <span class="extra-row__label">${e.name}</span>
              <span class="extra-row__price">+${formatPrice(e.price)}</span>
            </div>
            <div class="extra-row__controls">
              <button class="qty-btn qty-btn--minus" onclick="updateExtra('${e.name}', -1)">−</button>
              <span class="qty-value">${modalState.extras[e.name] || 0}</span>
              <button class="qty-btn qty-btn--plus" onclick="updateExtra('${e.name}', 1)">+</button>
            </div>
          </div>
        `).join("")}
      ` : ""}

      <h3 class="modal__section-title">Observação</h3>
      <textarea
        id="modal-observation"
        class="observation-textarea"
        placeholder="Ex: sem cebola, bem passado..."
        rows="2"
        oninput="modalState.observation = this.value"
      >${modalState.observation || ""}</textarea>
    </div>
    <div class="modal__footer">
      <button class="btn-add" onclick="addToCart()">
        <span>Adicionar</span>
        <span class="btn-add__price">${formatPrice(getModalTotal())}</span>
      </button>
    </div>
  `;
}

function addToCart() {
  const p = modalState.product;
  const size = p.sizes[modalState.selectedSize];
  const selectedExtras = p.extras
    .filter(e => (modalState.extras[e.name] || 0) > 0)
    .map(e => ({ name: e.name, quantity: modalState.extras[e.name], price: e.price }));

  const item = {
    id: generateId(),
    productId: p.id,
    name: p.name,
    size: size.name,
    sizePrice: size.price,
    extras: selectedExtras,
    observation: modalState.observation || "",
    quantity: 1,
    image: p.image,
  };

  Cart.addItem(item);
  closeModal();
  updateCartBar();
}

// ===== Cart Bar =====
function updateCartBar() {
  const bar = document.getElementById("cart-bar");
  const count = Cart.getCount();
  const total = Cart.getTotal();

  if (count > 0) {
    bar.classList.add("visible");
    document.getElementById("cart-count").textContent = count;
    document.getElementById("cart-total").textContent = formatPrice(total);
  } else {
    bar.classList.remove("visible");
  }
}

// ===== Inicialização =====
document.addEventListener("DOMContentLoaded", () => {
  renderCategoryBar();
  renderMenu();
  setupCategoryObserver();
  updateCartBar();
  setActiveCategory(categories[0].id);

  // Fechar modal clicando no overlay
  document.getElementById("modal-overlay").addEventListener("click", (e) => {
    if (e.target === e.currentTarget) closeModal();
  });
});
