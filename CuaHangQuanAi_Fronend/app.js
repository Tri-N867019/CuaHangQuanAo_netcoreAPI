/* ==========================================================================
   NT CLOTHING - CORE FRONTEND LOGIC (app.js)
   1. CẬU HÌNH & XÁC THỰC (CONFIG & AUTH)
   2. SẢN PHẨM & BỘ LỌC (PRODUCTS & FILTERS)
   3. GIAO DIỆN CHUNG (HEADER & FOOTER)
   ========================================================================== */

// 1. CẤU HÌNH & XÁC THỰC
const API_BASE_URL = 'https://localhost:7197';

function authFetch(url, options = {}) {
    const fullUrl = (url.startsWith('/api') || url.startsWith('api')) 
        ? `${API_BASE_URL}${url.startsWith('/') ? '' : '/'}${url}` 
        : url;

    const token = localStorage.getItem('token');
    const headers = { ...options.headers };
    if (token) headers['Authorization'] = `Bearer ${token}`;

    return fetch(fullUrl, { ...options, headers });
}


// === HỆ THỐNG THÔNG BÁO TOAST (THAY THẾ ALERT) ===
function showToast(message, type = 'success') {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = `toast-item ${type}`;
    
    // Icon mapping
    const icons = {
        success: 'bi-check-circle-fill text-success',
        error: 'bi-x-circle-fill text-danger',
        warning: 'bi-exclamation-triangle-fill text-warning',
        info: 'bi-info-circle-fill text-info'
    };
    const iconClass = icons[type] || icons.info;

    toast.innerHTML = `
        <i class="bi ${iconClass}"></i>
        <div class="toast-message">${message}</div>
        <button class="toast-close" onclick="this.parentElement.remove()">&times;</button>
    `;

    container.appendChild(toast);

    // Tự động xóa sau 5 giây
    setTimeout(() => {
        toast.classList.add('hide');
        setTimeout(() => toast.remove(), 400);
    }, 5000);
}

// Ghi đè alert() mặc định để tự động dùng Toast
window.alert = function(msg) {
    // Nhận diện một số từ khóa để đổi loại thông báo
    let type = 'info';
    if (msg.toLowerCase().includes('thành công') || msg.toLowerCase().includes('cảm ơn')) type = 'success';
    if (msg.toLowerCase().includes('lỗi') || msg.toLowerCase().includes('thất bại') || msg.toLowerCase().includes('không')) type = 'error';
    if (msg.toLowerCase().includes('cảnh báo') || msg.toLowerCase().includes('chú ý') || msg.toLowerCase().includes('hết')) type = 'warning';
    
    showToast(msg, type);
};


// 2. SẢN PHẨM & BỘ LỌC
function layDanhSachSanPham() {
    const urlParamsSearch = new URLSearchParams(window.location.search);
    const searchKeyword = urlParamsSearch.get('search');
    let apiUrl = `${API_BASE_URL}/api/SanPham`;
    if (searchKeyword) apiUrl += `?search=${encodeURIComponent(searchKeyword)}`;

    authFetch(apiUrl)
        .then(res => res.json())
        .then(data => {
            const up = new URLSearchParams(window.location.search);
            const loaiId = up.get('loaiId');
            const gioiTinh = up.get('gioiTinh');
            const minPrice = up.get('minPrice');
            const maxPrice = up.get('maxPrice');
            const isSale = up.get('isSale');
            const thuongHieuId = up.get('thuongHieuId');
            const mauId = up.get('mauId');

            let dsHienThi = data;
            
            // Bộ lọc dữ liệu
            if (isSale === 'true') dsHienThi = dsHienThi.filter(sp => sp.khuyenMai > 0);
            if (minPrice) dsHienThi = dsHienThi.filter(sp => sp.giaBan >= Number(minPrice));
            if (maxPrice) dsHienThi = dsHienThi.filter(sp => sp.giaBan <= Number(maxPrice));
            if (loaiId) dsHienThi = dsHienThi.filter(sp => sp.loaiId == loaiId);
            if (thuongHieuId) dsHienThi = dsHienThi.filter(sp => sp.thuongHieuId == thuongHieuId);
            if (mauId) dsHienThi = dsHienThi.filter(sp => sp.mauIds?.includes(Number(mauId)));

            // Giới hạn 4 SP nếu ở trang chủ
            if (window.location.pathname.endsWith('index.html')) dsHienThi = dsHienThi.slice(0, 4);

            // Cập nhật số lượng SP
            const lblCount = document.getElementById('product-count');
            if (lblCount) lblCount.innerText = `${dsHienThi.length} sản phẩm`;

            // Active Filters UI
            renderActiveFilters(data, up);

            const container = document.getElementById('danh-sach-san-pham');
            if (!container) return;
            container.innerHTML = dsHienThi.length ? '' : '<div class="w-100 text-center mt-5"><p class="text-secondary">Không tìm thấy sản phẩm phù hợp.</p></div>';

            dsHienThi.forEach(sp => {
                const anh = sp.hinhAnh?.startsWith('/') ? API_BASE_URL + sp.hinhAnh : (sp.hinhAnh || 'images/no-image.png');
                const km = sp.khuyenMai || 0;
                const isSaleItem = km > 0;
                const giaGiam = sp.giaBan - km;
                const isOutOfStock = (sp.soLuong ?? 0) <= 0;

                const badge = isSaleItem 
                    ? `<div class="position-absolute top-0 start-0 bg-danger text-white px-2 py-1 fs-10 fw-bold z-1">-${((km/sp.giaBan)*100).toFixed(0)}%</div>`
                    : (sp.id % 2 === 0 ? `<div class="position-absolute top-0 start-0 bg-dark text-white px-2 py-1 fs-10 fw-bold z-1">NEW</div>` : '');

                const outOfStockOverlay = isOutOfStock 
                    ? `<div class="out-of-stock-badge">HẾT HÀNG</div>` 
                    : '';

                const giaHtml = isSaleItem
                    ? `<span class="text-muted text-decoration-line-through me-2 fs-13">${sp.giaBan.toLocaleString()}đ</span><span class="text-danger fw-bold fs-15">${giaGiam.toLocaleString()}đ</span>`
                    : `<span class="text-dark fw-bold fs-14">${sp.giaBan.toLocaleString()}đ</span>`;

                container.innerHTML += `
                    <div class="col-md-4 mb-4">
                        <div class="product-item position-relative">
                            ${badge}
                            <div class="product-img-wrap bg-light cursor-pointer ${isOutOfStock ? 'out-of-stock' : ''}" onclick="location.href='chi-tiet-sp.html?id=${sp.id}'">
                                <img src="${anh}" class="img-fluid w-100 aspect-3-4" alt="${sp.ten}">
                                ${outOfStockOverlay}
                                <div class="position-absolute bottom-0 end-0 p-2">
                                    <button class="btn btn-light btn-sm border d-flex align-items-center justify-content-center" style="width:22px;height:22px"><i class="bi bi-plus-lg fs-12"></i></button>
                                </div>
                            </div>
                            <div class="product-info text-center mt-3">
                                <p class="text-uppercase mb-1 text-secondary fs-11 fw-bold">${sp.tenThuongHieu || 'Khác'}</p>
                                <h6 class="text-dark mb-1 fs-14 fw-400">${sp.ten}</h6>
                                <div class="d-flex justify-content-center gap-1 mb-2">
                                    ${(sp.maMaus || []).map(m => `<span class="dot-8 rounded-circle border" style="background:${m}"></span>`).join('')}
                                </div>
                                <p class="m-0">${giaHtml}</p>
                            </div>
                        </div>
                    </div>`;
            });
        }).catch(err => console.error(err));
}

function renderActiveFilters(allData, up) {
    const container = document.getElementById('active-filters');
    if (!container) return;
    container.innerHTML = '';
    let hasFilter = false;

    const addTag = (label, keys) => {
        hasFilter = true;
        const tag = document.createElement('span');
        tag.className = 'border px-2 py-1 fs-12 d-flex align-items-center gap-1 bg-white text-secondary';
        tag.innerHTML = `${label} <i class="bi bi-x cursor-pointer fs-5"></i>`;
        tag.querySelector('i').onclick = () => {
            const url = new URL(location.href);
            (Array.isArray(keys) ? keys : [keys]).forEach(k => url.searchParams.delete(k));
            location.href = url.toString();
        };
        container.appendChild(tag);
    };

    if (up.get('isSale') === 'true') addTag('Hàng Sale', 'isSale');
    if (up.get('minPrice') && up.get('maxPrice')) addTag(`${Number(up.get('minPrice')).toLocaleString()}đ - ${Number(up.get('maxPrice')).toLocaleString()}đ`, ['minPrice', 'maxPrice']);
    if (up.get('search')) addTag(`Tìm: "${up.get('search')}"`, 'search');
    if (up.get('thuongHieuId')) {
        const item = allData.find(x => x.thuongHieuId == up.get('thuongHieuId'));
        addTag(`Brand: ${item?.tenThuongHieu || up.get('thuongHieuId')}`, 'thuongHieuId');
    }
    if (up.get('loaiId')) {
        const item = allData.find(x => x.loaiId == up.get('loaiId'));
        addTag(`Loại: ${item?.tenLSP || up.get('loaiId')}`, 'loaiId');
    }

    if (hasFilter) {
        const clear = document.createElement('a');
        clear.href = 'san-pham.html';
        clear.className = 'text-dark fw-bold fs-13 ms-3';
        clear.innerText = 'XOÁ BỘ LỌC';
        container.appendChild(clear);
    }
}

// 3. GIAO DIỆN CHUNG
async function loadHeaderFooter() {
    try {
        const h = await authFetch('header.html'); document.getElementById('app-header').innerHTML = await h.text();
        const f = await authFetch('footer.html'); document.getElementById('app-footer').innerHTML = await f.text();

        layThuongHieuChoHeader();
        layLoaiSPChoHeader();
        layDanhSachMauSac();
        kiemTraDangNhap();

        // Xử lý Tìm kiếm
        const formTk = document.getElementById('form-timkiem');
        if (formTk) {
            const inputTk = document.getElementById('input-timkiem');
            inputTk.value = new URLSearchParams(location.search).get('search') || '';
            formTk.onsubmit = (e) => {
                e.preventDefault();
                const v = inputTk.value.trim();
                location.href = v ? `san-pham.html?search=${encodeURIComponent(v)}` : 'san-pham.html';
            };
        }

        // Xử lý Liên hệ & Footer
        const btnSub = document.getElementById('btn-subscribe');
        if (btnSub) {
            btnSub.onclick = () => {
                const mail = document.getElementById('footer-email-input').value;
                if (mail.includes('@')) alert(`Cảm ơn! Chúng tôi sẽ liên hệ qua ${mail}`);
                else alert('Email không hợp lệ!');
            };
        }

        const hotline = document.getElementById('hotline-click');
        if (hotline) hotline.onclick = () => {
            navigator.clipboard.writeText("0365075600").then(() => alert('Đã copy Hotline: 0365075600'));
        };

    } catch (err) { console.error('Lỗi layout:', err); }
}

function layThuongHieuChoHeader() {
    authFetch('/api/ThuongHieu')
        .then(res => res.json())
        .then(data => {
            const menu = document.getElementById('menu-thuonghieu-header');
            if (menu) menu.innerHTML = data.map(th => `<li><a class="dropdown-item" href="san-pham.html?thuongHieuId=${th.id}">${th.tenTH}</a></li>`).join('');

            const sidebar = document.getElementById('filter-brands');
            if (sidebar) {
                const cur = new URLSearchParams(location.search).get('thuongHieuId');
                sidebar.innerHTML = data.map(th => `
                    <div class="form-check mb-2">
                        <input class="form-check-input filter-checkbox" type="checkbox" value="${th.id}" id="brand${th.id}" data-type="thuongHieuId" ${cur == th.id ? 'checked' : ''}>
                        <label class="form-check-label text-secondary fs-13" for="brand${th.id}">${th.tenTH}</label>
                    </div>`).join('');
                initFilterListeners();
            }
        });
}

function layLoaiSPChoHeader() {
    authFetch('/api/LoaiSP')
        .then(res => res.json())
        .then(data => {
            const menu = document.getElementById('menu-danhmuc-header');
            if (menu) {
                menu.querySelectorAll('.dynamic-category').forEach(el => el.remove());
                const groups = { 'ÁO': [], 'QUẦN': [] };
                data.forEach(loai => {
                    const ten = loai.tenLSP.toUpperCase();
                    if (ten.includes('ÁO')) groups['ÁO'].push(loai);
                    else if (ten.includes('QUẦN')) groups['QUẦN'].push(loai);
                });

                for (const [name, items] of Object.entries(groups)) {
                    if (!items.length) continue;
                    const li = document.createElement('li'); li.className = 'nav-item dropdown dynamic-category';
                    li.innerHTML = `
                        <a class="nav-link text-dark fw-bold dropdown-toggle px-3" href="#" id="drop${name}" data-bs-toggle="dropdown">${name}</a>
                        <ul class="dropdown-menu border-0 shadow-sm">${items.map(i => `<li><a class="dropdown-item py-2" href="san-pham.html?loaiId=${i.id}">${i.tenLSP}</a></li>`).join('')}</ul>`;
                    menu.appendChild(li);
                }
            }

            const sidebar = document.getElementById('filter-categories');
            if (sidebar) {
                const cur = new URLSearchParams(location.search).get('loaiId');
                sidebar.innerHTML = data.map(c => `
                    <div class="col-12">
                        <div class="form-check mb-2">
                            <input class="form-check-input filter-checkbox" type="checkbox" value="${c.id}" id="cat${c.id}" data-type="loaiId" ${cur == c.id ? 'checked' : ''}>
                            <label class="form-check-label text-secondary fs-13" for="cat${c.id}">${c.tenLSP}</label>
                        </div>
                    </div>`).join('');
                initFilterListeners();
            }
        });
}

function layDanhSachMauSac() {
    const container = document.getElementById('filter-colors'); if (!container) return;
    authFetch('/api/MauSac')
        .then(res => res.json())
        .then(data => {
            const unique = []; const seen = new Set();
            data.forEach(m => { if (!seen.has(m.maMau.toLowerCase())) { seen.add(m.maMau.toLowerCase()); unique.push(m); } });

            const cur = new URLSearchParams(location.search).get('mauId');
            container.innerHTML = '';
            unique.forEach(m => {
                const div = document.createElement('div');
                div.className = `border rounded-circle cursor-pointer ${cur == m.id ? 'border-dark border-3' : 'border-light'}`;
                div.style = `width:28px;height:28px;background:${m.maMau}`;
                div.title = m.tenMau;
                div.setAttribute('data-bs-toggle', 'tooltip');
                div.setAttribute('data-bs-placement', 'top');
                div.onclick = () => {
                    const url = new URL(location.href);
                    if (cur == m.id) url.searchParams.delete('mauId'); else url.searchParams.set('mauId', m.id);
                    location.href = url.toString();
                };
                container.appendChild(div);
            });
            // Khởi tạo tooltip nếu có
            if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
                const tooltips = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
                tooltips.map(t => new bootstrap.Tooltip(t));
            }
        });
}


// ======================== CHI TIẾT SẢN PHẨM ========================
/* ==========================================================================
   4. CHI TIẾT SẢN PHẨM & BIẾN THỂ (PRODUCT DETAIL)
   ========================================================================== */
function layChiTietSanPham(id) {
    const container = document.getElementById('product-detail-container');
    if (!container) return;

    authFetch(`/api/SanPham/chitiet/${id}`)
        .then(res => res.json())
        .then(sp => {
            document.getElementById('breadcrumb-name').innerText = sp.ten;
            document.title = `${sp.ten} - NT Clothing`;

            const sizes = [...new Map(sp.cacBienThe.map(b => [b.tenSize, b])).values()].filter(s => s.tenSize !== "undefined" && s.tenSize !== null);
            const colors = [...new Map(sp.cacBienThe.map(b => [b.tenMau, b])).values()].filter(c => c.tenMau !== "undefined" && c.tenMau !== null);

            let galleryHtml = sp.danhSachAnh?.length 
                ? `<div class="mb-3 bg-light d-flex justify-content-center border" style="height:550px">
                        <img id="main-product-image" src="${sp.danhSachAnh[0]}" class="img-fluid h-100 object-fit-contain cursor-zoom" alt="${sp.ten}">
                   </div>
                   <div class="d-flex gap-2 flex-wrap">
                        ${sp.danhSachAnh.map((img, i) => `<img src="${img}" class="thumbnail-img border rounded ${i === 0 ? 'active' : ''}" style="width:70px;height:90px;object-fit:cover" onclick="changeMainImage(this,'${img}')">`).join('')}
                   </div>`
                : `<div class="p-5 bg-light text-center border text-secondary h-100 d-flex flex-column justify-content-center"><i class="bi bi-image fs-60"></i><p>Chưa có hình ảnh</p></div>`;

            const sizeHtml = sizes.length 
                ? sizes.map(s => {
                    const isSizeOutOfStock = sp.cacBienThe.filter(b => b.tenSize === s.tenSize).every(b => (b.soLuongTon ?? 0) <= 0);
                    return `<button class="variant-btn me-2 mb-2 ${isSizeOutOfStock ? 'disabled' : ''}" 
                            onclick="${isSizeOutOfStock ? '' : `selectVariant(this,'size')`}" 
                            data-size="${s.tenSize}" ${isSizeOutOfStock ? 'disabled' : ''}>${s.tenSize}</button>`;
                }).join('') 
                : '<p class="fst-italic text-secondary fs-14">Freesize</p>';

            const colorHtml = colors.length 
                ? colors.map(c => {
                    const isColorOutOfStock = sp.cacBienThe.filter(b => b.tenMau === c.tenMau).every(b => (b.soLuongTon ?? 0) <= 0);
                    const code = c.maMau || "#ccc";
                    return `
                        <button class="variant-btn me-2 mb-2 d-inline-flex align-items-center ${isColorOutOfStock ? 'disabled' : ''}" 
                                onclick="${isColorOutOfStock ? '' : `selectVariant(this,'color')`}" 
                                data-color="${c.tenMau}" ${isColorOutOfStock ? 'disabled' : ''}>
                            <span class="dot-12 rounded-circle me-2 border" style="background:${code}; width:12px; height:12px; display:inline-block;"></span>
                            <span>${c.tenMau}</span>
                        </button>`;
                }).join('') 
                : '<p class="fst-italic text-secondary fs-14">Một màu duy nhất</p>';

            const km = sp.khuyenMai || 0;
            const giaHtml = km > 0 
                ? `<span class="text-secondary text-decoration-line-through me-3 fs-18">${sp.giaBan.toLocaleString()}đ</span><span class="fs-4 fw-bolder text-danger">${(sp.giaBan - km).toLocaleString()}đ</span>`
                : `<span class="fs-4 fw-bolder text-danger">${sp.giaBan?.toLocaleString() || 'Liên hệ'}đ</span>`;

            window.currentProductVariants = sp.cacBienThe;
            window.selectedColor = window.selectedSize = null;

            const isAllOutOfStock = sp.cacBienThe.every(b => (b.soLuongTon ?? 0) <= 0);

            container.innerHTML = `
                <div class="col-lg-6 mb-4 pe-lg-5">${galleryHtml}</div>
                <div class="col-lg-6">
                    <p class="text-secondary fw-bold text-uppercase fs-13 mb-1">${sp.tenThuongHieu || 'Khác'}</p>
                    <h2 class="fw-bolder mb-3">${sp.ten}</h2>
                    <div class="mb-3">${giaHtml}</div>
                    <div class="border-top pt-3 mt-3"><p class="fs-14 text-dark mb-4 lh-1-6">${sp.moTa || 'Đang cập nhật mô tả...'}</p></div>
                    <div class="mb-4"><h6 class="fw-bold fs-14 mb-2">MÀU SẮC</h6><div id="color-options">${colorHtml}</div></div>
                    <div class="mb-4"><h6 class="fw-bold fs-14 mb-2">KÍCH CỠ</h6><div id="size-options">${sizeHtml}</div></div>
                    <div class="d-flex mb-4 border-top border-bottom py-3">
                        <div class="input-group me-3 border w-130 rounded-0">
                            <button class="btn btn-light rounded-0 px-3" onclick="let i=document.getElementById('qty'); if(i.value>1) i.value--">-</button>
                            <input type="text" class="form-control text-center border-0 bg-light fw-bold" id="qty" value="1" readonly>
                            <button class="btn btn-light rounded-0 px-3" onclick="let i=document.getElementById('qty'); i.value++">+</button>
                        </div>
                        <button id="btn-add-to-cart" class="btn btn-dark flex-grow-1 rounded-0 fw-bold py-2" 
                                ${isAllOutOfStock ? 'disabled' : ''} 
                                onclick="themVaoGio(${sp.id})">
                            ${isAllOutOfStock ? 'HẾT HÀNG' : 'THÊM VÀO GIỎ HÀNG'}
                        </button>
                    </div>
                </div>`;
        }).catch(err => console.error(err));
}

function changeMainImage(el, src) {
    document.getElementById('main-product-image').src = src;
    document.querySelectorAll('.thumbnail-img').forEach(img => img.classList.remove('active'));
    el.classList.add('active');
}

function selectVariant(el, type) {
    el.parentElement.querySelectorAll('.variant-btn').forEach(btn => btn.classList.remove('active'));
    el.classList.add('active');
    if (type === 'color') window.selectedColor = el.dataset.color;
    else window.selectedSize = el.dataset.size;

    // Kiểm tra tồn kho của biến thể sau khi chọn
    const varList = window.currentProductVariants || [];
    const hasC = varList.some(v => v.tenMau || v.TenMau);
    const hasS = varList.some(v => v.tenSize || v.TenSize);

    if ((hasC && window.selectedColor) && (hasS && window.selectedSize)) {
        const found = varList.find(v => (v.tenMau === window.selectedColor) && (v.tenSize === window.selectedSize));
        const btnAdd = document.getElementById('btn-add-to-cart');
        if (found && (found.soLuongTon ?? 0) <= 0) {
            btnAdd.disabled = true;
            btnAdd.innerText = 'BẢN BIẾN THỂ NÀY HẾT HÀNG';
            btnAdd.classList.replace('btn-dark', 'btn-secondary');
        } else {
            btnAdd.disabled = false;
            btnAdd.innerText = 'THÊM VÀO GIỎ HÀNG';
            btnAdd.classList.replace('btn-secondary', 'btn-dark');
        }
    }
}

/* ==========================================================================
   5. GIỎ HÀNG (SHOPPING CART)
   ========================================================================== */
async function themVaoGio(productId) {
    const token = localStorage.getItem('token');
    const varList = window.currentProductVariants || [];
    const hasC = varList.some(v => v.tenMau || v.TenMau || v.maMau || v.MaMau);
    const hasS = varList.some(v => v.tenSize || v.TenSize);

    if ((hasC && !window.selectedColor) || (hasS && !window.selectedSize)) return alert("Vui lòng chọn Màu sắc/Kích cỡ!");

    const found = varList.find(v => {
        const vColor = v.tenMau || v.TenMau || v.maMau || v.MaMau;
        const vSize = v.tenSize || v.TenSize;
        return (!hasC || vColor === window.selectedColor) && (!hasS || vSize === window.selectedSize);
    });
    if (!found && (hasC || hasS)) return alert("Lựa chọn này hiện đang hết hàng!");

    const variantId = found ? found.id : productId;
    const stock = found ? (found.soLuongTon ?? 0) : 0;
    const qty = parseInt(document.getElementById('qty')?.value || 1);

    if (stock <= 0) return alert("Sản phẩm này hiện đang hết hàng!");
    if (qty > stock) return alert(`Chỉ còn ${stock} sản phẩm trong kho!`);

    if (token) {
        try {
            const res = await authFetch('/api/GioHang', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ sanPhamBienTheId: variantId, soLuong: qty })
            });
            if (res.ok) (alert('Đã thêm vào giỏ!'), updateCartBadge());
            else alert('Lỗi: ' + await res.text());
        } catch (err) { console.error(err); }
    } else {
        const cart = getCart(); const exist = cart.find(i => i.id === variantId);
        if (exist) exist.qty += qty; else cart.push({ id: variantId, qty });
        setCart(cart); updateCartBadge(); alert('Đã lưu vào giỏ hàng (Máy khách)!');
    }
}

async function syncLocalCartToBackend() {
    const token = localStorage.getItem('token'); if (!token) return;
    const local = getCart(); if (!local.length) return;

    for (const item of local) {
        await authFetch('/api/GioHang', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ sanPhamBienTheId: item.id, soLuong: item.qty })
        });
    }
    localStorage.removeItem('cart'); updateCartBadge();
}

/* ==========================================================================
   6. CƠ CHẾ XÁC THỰC (AUTHENTICATION)
   ========================================================================== */
function parseJwt(token) {
    try {
        const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
        const json = decodeURIComponent(atob(base64).split('').map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));
        return JSON.parse(json);
    } catch (e) { return null; }
}

function getCart() { try { return JSON.parse(localStorage.getItem('cart') || '[]'); } catch (e) { return []; } }
function setCart(cart) { localStorage.setItem('cart', JSON.stringify(cart)); }

async function updateCartBadge() {
    const token = localStorage.getItem('token');
    let total = 0;
    if (token) {
        try {
            const res = await authFetch('/api/GioHang');
            if (res.ok) { const data = await res.json(); total = data.reduce((s, i) => s + (i.soLuong || 0), 0); }
        } catch (err) { console.error(err); }
    } else {
        total = getCart().reduce((s, i) => s + (i.qty || 0), 0);
    }
    document.querySelectorAll('.cart-count-badge').forEach(s => s.innerText = total);
}

function saveUserSessionFromToken(token) {
    const user = parseJwt(token); if (!user) return null;
    const sid = user.sub || user.nameid || null;
    // Check both standard claim and Microsoft identity claim
    const role = user.role || user["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || 'KhachHang';
    const name = user.unique_name || user.name || 'User';

    if (sid) localStorage.setItem('userId', sid);
    localStorage.setItem('role', role);
    localStorage.setItem('username', name);
    return { sid, role, name };
}

function attachUserMenuActions() {
    const container = document.getElementById('user-menu-container'); if (!container) return;
    container.onclick = (e) => {
        const item = e.target.closest('[data-action]'); if (!item) return;
        const act = item.dataset.action; e.preventDefault();
        
        if (act === 'login') location.href = 'dang-nhap.html';
        else if (act === 'register') location.href = 'dang-ky.html';
        else if (act === 'profile') location.href = 'thong-tin-tai-khoan.html#profile';
        else if (act === 'history') location.href = 'thong-tin-tai-khoan.html#history';
        else if (act === 'profile-modal') showAccountInfoModal();
        else if (act === 'cart') location.href = 'gio-hang.html';
        else if (act === 'logout') dangXuat();
    };
}

function kiemTraDangNhap() {
    const token = localStorage.getItem('token');
    const container = document.getElementById('user-menu-container'); if (!container) return;
    
    if (token) {
        const user = parseJwt(token);
        if (user && user.exp * 1000 > Date.now()) {
            const session = saveUserSessionFromToken(token);
            // Include "Nhân viên" with accents and other variations for robustness
            const isAdmin = ['Admin1', 'Admin', 'NhanVien', 'Nhân viên', 'QuanLyHeThong'].includes(session.role);
            
            container.className = 'ms-lg-4 dropdown user-header-box';
            // Hiển thị trực tiếp Tên và Chức vụ ra Header để "thu nhỏ hay zoom đều hiện"
            container.innerHTML = `
                <a class="text-dark d-flex align-items-center gap-2 p-1 text-decoration-none dropdown-toggle border-0" href="#" id="dropUser" data-bs-toggle="dropdown" aria-expanded="false">
                    <div class="user-avatar-small bg-dark text-white rounded-circle d-flex align-items-center justify-content-center fw-bold fs-12">${session.name.charAt(0).toUpperCase()}</div>
                    <div class="text-start lh-1">
                        <div class="fw-bold fs-13 mb-0 text-truncate" style="max-width:110px">${session.name}</div>
                        <span class="fs-10 text-secondary text-uppercase fw-800">${session.role || 'Thành viên'}</span>
                    </div>
                </a>
                <ul class="dropdown-menu dropdown-menu-end shadow-lg border-0 mt-2" aria-labelledby="dropUser">
                    ${isAdmin ? '<li><a class="dropdown-item py-2 fw-bold text-primary" href="admin.html"><i class="bi bi-shield-lock-fill me-2"></i>Quản trị hệ thống</a></li>' : ''}
                    <li><a class="dropdown-item py-2" data-action="profile-modal" href="#"><i class="bi bi-person-badge-fill me-2"></i>Thông tin chi tiết</a></li>
                    <li><a class="dropdown-item py-2" data-action="profile" href="#"><i class="bi bi-gear-fill me-2"></i>Cài đặt tài khoản</a></li>
                    <li><a class="dropdown-item py-2" data-action="history" href="#"><i class="bi bi-bag-check-fill me-2"></i>Lịch sử mua sắm</a></li>
                    <li><hr class="dropdown-divider"></li>
                    <li><a class="dropdown-item py-2 text-danger fw-bold" data-action="logout" href="#"><i class="bi bi-box-arrow-right me-2"></i>Đăng xuất</a></li>
                </ul>`;
            attachUserMenuActions();
            return;
        }
    }
    
    container.innerHTML = `<a href="dang-nhap.html" class="text-dark fs-14 fw-bold d-flex align-items-center gap-2 text-decoration-none">
        <i class="bi bi-box-arrow-in-right fs-4"></i>
        <span>Đăng nhập</span>
    </a>`;
}

// Hàm mở Modal thông tin tài khoản (Chống zoom cực tốt)
async function showAccountInfoModal() {
    const old = document.getElementById('accountInfoModal'); if (old) old.remove();
    try {
        const res = await authFetch('/api/User/profile');
        if (!res.ok) return alert('Vui lòng đăng nhập lại');
        const u = await res.json();
        
        const html = `
            <div class="modal fade" id="accountInfoModal" tabindex="-1">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content border-0 shadow-lg" style="border-radius: 20px;">
                        <div class="modal-header bg-dark text-white border-0 py-3" style="border-radius: 20px 20px 0 0;">
                            <h5 class="modal-title fw-bold">THÀNH VIÊN NT CLOTHING</h5>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body p-4 text-center">
                            <div class="bg-light rounded-circle d-inline-flex align-items-center justify-content-center mb-3 shadow-sm" style="width:80px;height:80px; font-size: 32px; font-weight:800; color:#333">
                                ${u.tenDangNhap?.charAt(0).toUpperCase() || 'U'}
                            </div>
                            <h4 class="fw-bolder mb-1">${u.hoVaTen || u.tenDangNhap}</h4>
                            <p class="text-secondary fs-13 mb-4"><i class="bi bi-shield-check me-1"></i>${u.tenQuyen || 'Người dùng hệ thống'}</p>
                            
                            <div class="text-start bg-light p-3 rounded-3 mb-3">
                                <div class="row g-3">
                                    <div class="col-6"><p class="mb-0 text-muted fs-11 text-uppercase fw-bold">Số điện thoại</p><p class="fw-bold mb-0 fs-14">${u.sdt || 'Chưa cập nhật'}</p></div>
                                    <div class="col-6"><p class="mb-0 text-muted fs-11 text-uppercase fw-bold">Email</p><p class="fw-bold mb-0 fs-14 text-truncate">${u.email || 'Chưa cập nhật'}</p></div>
                                    <div class="col-12"><p class="mb-0 text-muted fs-11 text-uppercase fw-bold">Địa chỉ giao hàng</p><p class="fw-bold mb-0 fs-14">${u.diaChi || 'Chưa có địa chỉ mặc định'}</p></div>
                                </div>
                            </div>
                            
                            <div class="d-grid gap-2">
                                <button class="btn btn-dark fw-bold py-2" data-bs-dismiss="modal" onclick="location.href='thong-tin-tai-khoan.html'">CHỈNH SỬA HỒ SƠ</button>
                                <button class="btn btn-outline-secondary btn-sm border-0 mt-2" data-bs-dismiss="modal">Đóng</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>`;
        document.body.insertAdjacentHTML('beforeend', html);
        new bootstrap.Modal(document.getElementById('accountInfoModal')).show();
    } catch (e) { alert('Lỗi kết nối dữ liệu'); }
}

/* ==========================================================================
   7. HỒ SƠ NGƯỜI DÙNG (USER PROFILE)
   ========================================================================== */
async function loadProfilePage() {
    const container = document.getElementById('profile-info-content'); if (!container) return;
    const token = localStorage.getItem('token');
    if (!token) return container.innerHTML = '<div class="alert alert-warning">Vui lòng <a href="dang-nhap.html">đăng nhập</a> để xem hồ sơ.</div>';

    try {
        const res = await authFetch('/api/User/profile');
        if (!res.ok) { if (res.status === 401) dangXuat(); throw new Error('Lỗi tải hồ sơ'); }
        const user = await res.json();
        
        // Cập nhật Sidebar
        const nameEl = document.getElementById('profile-name'); if (nameEl) nameEl.innerText = user.hoVaTen || 'Người dùng';
        const roleEl = document.getElementById('profile-role'); if (roleEl) roleEl.innerText = user.role || 'Thành viên';
        const imgEl = document.getElementById('profile-img-large'); if (imgEl && user.avatar) imgEl.src = user.avatar;

        // Cập nhật nội dung chi tiết
        container.innerHTML = `
            <div class="row g-4">
                <div class="col-md-6"><div class="p-3 border rounded bg-light"><small class="text-muted d-block mb-1 text-uppercase fw-bold fs-10">Họ tên</small><div class="fw-bold text-dark">${user.hoVaTen || '---'}</div></div></div>
                <div class="col-md-6"><div class="p-3 border rounded bg-light"><small class="text-muted d-block mb-1 text-uppercase fw-bold fs-10">Email</small><div class="fw-bold text-dark">${user.email || '---'}</div></div></div>
                <div class="col-md-6"><div class="p-3 border rounded bg-light"><small class="text-muted d-block mb-1 text-uppercase fw-bold fs-10">Số điện thoại</small><div class="fw-bold text-dark">${user.sdt || '---'}</div></div></div>
                <div class="col-md-6"><div class="p-3 border rounded bg-light"><small class="text-muted d-block mb-1 text-uppercase fw-bold fs-10">Địa chỉ</small><div class="fw-bold text-dark text-truncate">${user.diaChi || '---'}</div></div></div>
            </div>`;
    } catch (err) { container.innerHTML = '<div class="alert alert-danger">Lỗi kết nối.</div>'; }
}

function dangXuat() {
    localStorage.clear(); location.reload();
}

/* ==========================================================================
   8. TIỆN ÍCH & KHỞI TẠO (UTILS & INIT)
   ========================================================================== */
function formatCurrency(v) { return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v); }

function initFilterListeners() {
    document.querySelectorAll('.filter-checkbox').forEach(chk => {
        if (chk.dataset.listener) return;
        chk.onclick = () => {
            const url = new URL(location.href); const type = chk.dataset.type;
            if (chk.checked) {
                url.searchParams.set(type, chk.value);
                document.querySelectorAll(`.filter-checkbox[data-type="${type}"]`).forEach(o => { if (o !== chk) o.checked = false; });
            } else url.searchParams.delete(type);
            location.href = url.toString();
        };
        chk.dataset.listener = 'true';
    });
}

document.addEventListener("DOMContentLoaded", async () => {
    await loadHeaderFooter(); updateCartBadge(); layDanhSachSanPham(); loadProfilePage();
    
    // Filter Sale
    const chkSale = document.getElementById('filter-sale');
    if (chkSale) {
        chkSale.checked = new URLSearchParams(location.search).get('isSale') === 'true';
        chkSale.onchange = () => {
            const url = new URL(location.href);
            if (chkSale.checked) url.searchParams.set('isSale', 'true'); else url.searchParams.delete('isSale');
            location.href = url.toString();
        };
    }

    // Filter Price
    document.querySelectorAll('.price-filter').forEach(r => {
        const u = new URLSearchParams(location.search);
        if (r.value === `${u.get('minPrice') || ''}-${u.get('maxPrice') || ''}`) r.checked = true;
        r.onchange = () => {
            const [min, max] = r.value.split('-'); const url = new URL(location.href);
            if (min) url.searchParams.set('minPrice', min); else url.searchParams.delete('minPrice');
            if (max) url.searchParams.set('maxPrice', max); else url.searchParams.delete('maxPrice');
            location.href = url.toString();
        };
    });

    initFilterListeners();

    // Login Form
    const fLogin = document.getElementById('form-dangnhap');
    if (fLogin) {
        fLogin.onsubmit = async (e) => {
            e.preventDefault(); const btn = fLogin.querySelector('button'); const err = document.getElementById('login-error');
            const user = fLogin.username.value.trim();
            const pass = fLogin.password.value.trim();

            if (user.length < 3 || pass.length < 6) {
                err.innerText = 'Tên đăng nhập (min 3) và Mật khẩu (min 6) không hợp lệ!';
                err.style.display = 'block';
                return;
            }

            btn.disabled = true; btn.innerText = 'Đang xử lý...';
            try {
                const res = await authFetch('/api/Auth/login', {
                    method: 'POST', headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ tenDangNhap: fLogin.username.value, matKhau: fLogin.password.value })
                });
                const data = await res.json();
                if (res.ok && data.token) {
                    localStorage.setItem('token', data.token);
                    saveUserSessionFromToken(data.token);
                    await syncLocalCartToBackend();
                    location.href = 'index.html';
                } else { err.innerText = data.message || 'Sai thông tin!'; err.style.display = 'block'; }
            } catch (e) { err.innerText = 'Lỗi kết nối!'; err.style.display = 'block'; }
            btn.disabled = false; btn.innerText = 'ĐANG NHẬP';
        };
    }

    // Register Form
    const fReg = document.getElementById('form-dangky');
    if (fReg) {
        fReg.onsubmit = async (e) => {
            e.preventDefault(); const btn = fReg.querySelector('button'); const msg = document.getElementById('register-message');
            const data = {
                tenDangNhap: fReg['reg-username'].value.trim(),
                matKhau: fReg['reg-password'].value.trim(),
                email: fReg['reg-email'].value.trim(),
                hoVaTen: fReg['reg-fullname'].value.trim()
            };

            // Ràng buộc
            if (data.hoVaTen.length < 2) return (msg.innerText = 'Họ tên ít nhất 2 ký tự!', msg.className='text-danger', msg.style.display='block');
            if (!data.email.includes('@')) return (msg.innerText = 'Email không hợp lệ!', msg.className='text-danger', msg.style.display='block');
            if (data.tenDangNhap.length < 4 || data.tenDangNhap.includes(' ')) return (msg.innerText = 'Tên đăng nhập ít nhất 4 ký tự, không chứa khoảng trắng!', msg.className='text-danger', msg.style.display='block');
            if (data.matKhau.length < 6) return (msg.innerText = 'Mật khẩu ít nhất 6 ký tự!', msg.className='text-danger', msg.style.display='block');

            btn.disabled = true;
            try {
                const res = await authFetch('/api/Auth/register', {
                    method: 'POST', headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ ...data, phanQuyenId: 4 })
                });
                const resData = await res.json();
                if (res.ok) { msg.innerText = 'Thành công! Đang chuyển...'; msg.className = 'text-success'; setTimeout(() => location.href = 'dang-nhap.html', 1500); }
                else { msg.innerText = resData.message || 'Lỗi đăng ký!'; msg.className = 'text-danger'; }
                msg.style.display = 'block';
            } catch (e) { }
            btn.disabled = false;
        };
    }
});
/* ==========================================================================
   9. ĐƠN HÀNG & LỊCH SỬ (ORDERS & HISTORY)
   ========================================================================== */
async function loadOrderHistory() {
    const list = document.getElementById('order-history-body'); if (!list) return;
    try {
        const res = await authFetch('/api/HoaDon/lich-su');
        if (!res.ok) return list.innerHTML = '<tr><td colspan="5" class="text-center">Vui lòng đăng nhập.</td></tr>';
        const orders = await res.json();
        if (!orders.length) return list.innerHTML = '<tr><td colspan="5" class="text-center">Chưa có đơn hàng nào.</td></tr>';

        list.innerHTML = orders.map(o => {
            const status = ['Chờ duyệt', 'Đã duyệt', 'Đang giao', 'Hoàn thành', 'Đã hủy'][o.trangThai] || 'Không rõ';
            const color = ['warning', 'info', 'primary', 'success', 'danger'][o.trangThai] || 'secondary';
            return `
                <tr>
                    <td class="fw-bold">#${o.id}</td>
                    <td>${new Date(o.ngayTao).toLocaleDateString()}</td>
                    <td class="text-danger fw-bold">${o.tongTien.toLocaleString()}đ</td>
                    <td><span class="badge bg-${color}">${status}</span></td>
                    <td>
                        <button class="btn btn-sm btn-outline-dark me-1" onclick="viewOrderDetail(${o.id})">Chi tiết</button>
                        ${o.trangThai === 0 ? `<button class="btn btn-sm btn-outline-danger" onclick="cancelMyOrder(${o.id})">Hủy đơn</button>` : ''}
                    </td>
                </tr>`;
        }).join('');
    } catch (e) { console.error(e); }
}

async function cancelMyOrder(id) {
    if (!confirm("Bạn có chắc chắn muốn hủy đơn hàng này không?")) return;
    try {
        const res = await authFetch(`/api/HoaDon/${id}/cancel`, { method: 'PUT' });
        if (res.ok) {
            alert("Đã hủy đơn hàng thành công!");
            loadOrderHistory();
        } else {
            const err = await res.text();
            alert("Lỗi: " + err);
        }
    } catch (e) {
        console.error("Lỗi hủy đơn:", e);
        alert("Lỗi kết nối hệ thống.");
    }
}

async function viewOrderDetail(id) {
    const old = document.getElementById('orderDetailModal'); if (old) old.remove();
    try {
        const res = await authFetch(`/api/HoaDon/${id}`);
        const o = await res.json();
        const html = `
            <div class="modal fade" id="orderDetailModal" tabindex="-1">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content border-0 shadow-lg">
                        <div class="modal-header bg-dark text-white">
                            <h5 class="modal-title fw-bold">Chi Tiết Đơn Hàng #${o.id}</h5>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body p-4">
                            <div class="row mb-4">
                                <div class="col-md-6">
                                    <h6 class="fw-bold text-uppercase fs-12 text-secondary mb-2">Thông tin vận chuyển</h6>
                                    <p class="mb-1 fs-14">Người nhận: <strong>${o.tenNguoiNhan}</strong></p>
                                    <p class="mb-1 fs-14">SĐT: <strong>${o.sdtNhanHang}</strong></p>
                                    <p class="fs-13 text-muted">Địa chỉ: ${o.diaChiGiaoHang}</p>
                                </div>
                                <div class="col-md-6 text-md-end">
                                    <h6 class="fw-bold text-uppercase fs-12 text-secondary mb-2">Trạng thái</h6>
                                    <p class="mb-1 fs-14">Ngày đặt: ${new Date(o.ngayTao).toLocaleString()}</p>
                                    <p class="mb-0 fs-14">Thanh toán: <strong>${o.phuongThucThanhToan}</strong></p>
                                </div>
                            </div>
                            <table class="table table-bordered align-middle fs-14">
                                <thead class="bg-light"><tr><th>Sản phẩm</th><th class="text-center">Số lượng</th><th class="text-end">Đơn giá</th><th class="text-end">Thành tiền</th></tr></thead>
                                <tbody>
                                    ${o.chiTiet.map(i => `<tr><td><div class="fw-bold">${i.tenSanPham}</div><div class="fs-12 text-secondary">${i.tenMau} / ${i.tenSize}</div></td><td class="text-center">${i.soluong}</td><td class="text-end">${i.giaBan.toLocaleString()}đ</td><td class="text-end fw-bold">${i.thanhTien.toLocaleString()}đ</td></tr>`).join('')}
                                </tbody>
                                <tfoot>
                                    <tr><td colspan="3" class="text-end border-0">Phí ship:</td><td class="text-end border-0">${(o.phiVanChuyen || 0).toLocaleString()}đ</td></tr>
                                    <tr><td colspan="3" class="text-end border-0 fw-bold fs-16">TỔNG CỘNG:</td><td class="text-end border-0 fw-bold fs-16 text-danger">${o.tongTien.toLocaleString()}đ</td></tr>
                                </tfoot>
                            </table>
                        </div>
                    </div>
                </div>
            </div>`;
        document.body.insertAdjacentHTML('beforeend', html);
        new bootstrap.Modal(document.getElementById('orderDetailModal')).show();
    } catch (e) { alert('Không thể tải chi tiết đơn hàng'); }
}

/* ==========================================================================
   10. CHÍNH SÁCH & HỖ TRỢ (POLICIES)
   ========================================================================== */
function renderPolicyContent() {
    const box = document.getElementById('policy-content'); if (!box) return;
    const id = new URLSearchParams(location.search).get('id') || 'gioi-thieu';
    
    const data = {
        'gioi-thieu': { t: 'GIỚI THIỆU NT CLOTHING', c: '<p class="lead">NT Clothing - Nơi định hình phong cách quý ông hiện đại.</p><p>Sứ mệnh của chúng tôi là mang lại sự tự tin cho phái mạnh qua những thiết kế tối giản, tinh tế.</p>' },
        'giao-hang': { t: 'CHÍNH SÁCH GIAO HÀNG', c: '<p>Miễn phí vận chuyển cho đơn hàng từ 500.000đ.</p><ul class="fs-14"><li>Nội thành: 1-2 ngày</li><li>Tỉnh khác: 3-5 ngày</li></ul>' },
        'doi-hang': { t: 'CHÍNH SÁCH ĐỔI TRẢ', c: '<p>Hỗ trợ đổi trả trong vòng 30 ngày nếu sản phẩm còn nguyên tem mác. Miễn phí đổi hàng nếu lỗi do nhà sản xuất.</p>' },
        'bao-hanh': { t: 'CHÍNH SÁCH BẢO HÀNH', c: '<p>Bảo hành 90 ngày cho các lỗi đường chỉ, cúc áo, khóa kéo. Hỗ trợ sửa chữa miễn phí trọn đời cho các lỗi cơ bản.</p>' },
        'bao-mat': { t: 'CHÍNH SÁCH BẢO MẬT', c: '<p>Chúng tôi cam kết bảo mật tuyệt đối thông tin khách hàng. Thông tin của bạn chỉ được dùng để xử lý đơn hàng và cải thiện trải nghiệm mua sắm.</p>' },
        'vi-tri': { t: 'VỊ TRÍ CỬA HÀNG', c: '<p class="mb-3">NT Clothing hiện có mặt tại Tầng 1, Vincom Plaza Long Xuyên để phục vụ quý khách.</p><div class="ratio ratio-16x9 border shadow-sm"><iframe src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3924.473600120721!2d105.43248639678953!3d10.383908299999993!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x310a72e5cb479347:0x17df92ac4da33d5e!2sVincom Plaza Long Xuyên!5e0!3m2!1svi!2s!4v1776321759557!5m2!1svi!2s" style="border:0;" allowfullscreen="" loading="lazy" referrerpolicy="no-referrer-when-downgrade"></iframe></div>' },
        'dieu-khoan': { t: 'ĐIỀU KHOẢN DỊCH VỤ', c: '<p>Bằng việc truy cập website, bạn đồng ý với các điều khoản về thanh toán, bảo mật và trách nhiệm của người sử dụng tại NT Clothing.</p>' }
    }[id] || { t: 'THÔNG TIN', c: 'Đang cập nhật nội dung...' };

    box.innerHTML = `<h2>${data.t}</h2><div class="mt-4">${data.c}</div>`;
    document.querySelectorAll('.policy-sidebar a').forEach(a => {
        a.classList.remove('active'); if (a.id === 'link-' + id) a.classList.add('active');
    });
}

// Khởi tạo các trang đặc thù
// Khởi tạo các trang đặc thù
if (location.pathname.includes('thong-tin-tai-khoan.html')) {
    loadProfilePage();
    loadOrderHistory();

    // Xử lý Tab dựa trên Hash
    const hash = location.hash;
    if (hash === '#history') {
        const historyTab = document.getElementById('history-tab');
        if (historyTab) bootstrap.Tab.getOrCreateInstance(historyTab).show();
    } else {
        const profileTab = document.getElementById('profile-tab');
        if (profileTab) bootstrap.Tab.getOrCreateInstance(profileTab).show();
    }
}

// Actions for Profile
async function openEditProfileModal() {
    const old = document.getElementById('editProfileModal'); if (old) old.remove();
    try {
        const res = await authFetch('/api/User/profile');
        if (!res.ok) return alert('Không thể tải thông tin hồ sơ');
        const u = await res.json();

        const html = `
            <div class="modal fade" id="editProfileModal" tabindex="-1">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content border-0 shadow-lg">
                        <div class="modal-header bg-dark text-white">
                            <h5 class="modal-title fw-bold">CHỈNH SỬA HỒ SƠ</h5>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                        </div>
                        <form id="form-edit-profile">
                            <div class="modal-body p-4">
                                <div class="mb-3">
                                    <label class="form-label fs-13 text-uppercase fw-bold text-secondary">Họ và Tên</label>
                                    <input type="text" name="hoVaTen" class="form-control" value="${u.hoVaTen || ''}" required>
                                </div>
                                <div class="mb-3">
                                    <label class="form-label fs-13 text-uppercase fw-bold text-secondary">Số điện thoại</label>
                                    <input type="text" name="sdt" class="form-control" value="${u.sdt || ''}" required>
                                </div>
                                <div class="mb-3">
                                    <label class="form-label fs-13 text-uppercase fw-bold text-secondary">Địa chỉ</label>
                                    <textarea name="diaChi" class="form-control" rows="3">${u.diaChi || ''}</textarea>
                                </div>
                                <div id="edit-profile-msg" class="text-center"></div>
                            </div>
                            <div class="modal-footer border-0 pb-4">
                                <button type="button" class="btn btn-light" data-bs-dismiss="modal">Hủy</button>
                                <button type="submit" class="btn btn-dark px-4 fw-bold">LƯU THAY ĐỔI</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>`;
        document.body.insertAdjacentHTML('beforeend', html);
        const modalEl = document.getElementById('editProfileModal');
        const modal = new bootstrap.Modal(modalEl);
        modal.show();

        document.getElementById('form-edit-profile').onsubmit = async (e) => {
            e.preventDefault();
            const btn = e.target.querySelector('button[type="submit"]');
            const msg = document.getElementById('edit-profile-msg');
            btn.disabled = true; btn.innerText = 'Đang lưu...';

            const formData = new FormData(e.target);
            const dto = {
                hoVaTen: formData.get('hoVaTen').trim(),
                sdt: formData.get('sdt').trim(),
                diaChi: formData.get('diaChi').trim()
            };

            // Ràng buộc dữ liệu
            if (dto.hoVaTen.length < 2) {
                msg.innerHTML = '<div class="text-danger mb-2 small fw-bold">Họ tên phải có ít nhất 2 ký tự!</div>';
                btn.disabled = false; btn.innerText = 'LƯU THAY ĐỔI';
                return;
            }
            const phoneRegex = /^(0|84)(3|5|7|8|9)([0-9]{8})$/;
            if (!phoneRegex.test(dto.sdt)) {
                msg.innerHTML = '<div class="text-danger mb-2 small fw-bold">Số điện thoại không hợp lệ (10 số, bắt đầu bằng 0)!</div>';
                btn.disabled = false; btn.innerText = 'LƯU THAY ĐỔI';
                return;
            }

            try {
                const resUp = await authFetch('/api/User/cap-nhat-ho-so', {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(dto)
                });
                if (resUp.ok) {
                    msg.innerHTML = '<div class="text-success fw-bold mb-2">Cập nhật thành công!</div>';
                    localStorage.setItem('username', dto.hoVaTen); // Update local cache
                    loadProfilePage();
                    kiemTraDangNhap(); // Refresh header
                    setTimeout(() => modal.hide(), 1500);
                } else {
                    const errorMsg = await resUp.text();
                    msg.innerHTML = `<div class="text-danger mb-2">${errorMsg || 'Lỗi cập nhật'}</div>`;
                    btn.disabled = false; btn.innerText = 'LƯU THAY ĐỔI';
                }
            } catch (err) { alert('Lỗi kết nối'); btn.disabled = false; }
        };
    } catch (e) { alert('Lỗi dữ liệu'); }
}

function openChangePasswordModal() {
    const old = document.getElementById('changePasswordModal'); if (old) old.remove();
    const html = `
        <div class="modal fade" id="changePasswordModal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content border-0 shadow-lg" style="border-radius:15px">
                    <div class="modal-header bg-danger text-white border-0 py-3" style="border-radius:15px 15px 0 0">
                        <h5 class="modal-title fw-bold"><i class="bi bi-shield-lock me-2"></i>ĐỔI MẬT KHẨU</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <form id="form-change-password">
                        <div class="modal-body p-4">
                            <div class="mb-3">
                                <label class="form-label fs-12 text-uppercase fw-bold text-secondary">Mật khẩu cũ</label>
                                <input type="password" name="matKhauCu" class="form-control border-0 bg-light" placeholder="Nhập mật khẩu hiện tại" required>
                            </div>
                            <hr class="text-muted opacity-25">
                            <div class="mb-3">
                                <label class="form-label fs-12 text-uppercase fw-bold text-secondary">Mật khẩu mới</label>
                                <input type="password" name="matKhauMoi" class="form-control border-0 bg-light" placeholder="Ít nhất 6 ký tự" required minlength="6">
                            </div>
                            <div class="mb-3">
                                <label class="form-label fs-12 text-uppercase fw-bold text-secondary">Xác nhận mật khẩu mới</label>
                                <input type="password" name="confirmMatKhauMoi" class="form-control border-0 bg-light" placeholder="Nhập lại mật khẩu mới" required minlength="6">
                            </div>
                            <div id="change-pass-msg" class="text-center"></div>
                        </div>
                        <div class="modal-footer border-0 pb-4 justify-content-center">
                            <button type="button" class="btn btn-light px-4" data-bs-dismiss="modal">Hủy</button>
                            <button type="submit" class="btn btn-danger px-4 fw-bold">XÁC NHẬN ĐỔI</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>`;
    document.body.insertAdjacentHTML('beforeend', html);
    const modalEl = document.getElementById('changePasswordModal');
    const modal = new bootstrap.Modal(modalEl);
    modal.show();

    document.getElementById('form-change-password').onsubmit = async (e) => {
        e.preventDefault();
        const msg = document.getElementById('change-pass-msg');
        const formData = new FormData(e.target);
        
        const matKhauCu = formData.get('matKhauCu');
        const matKhauMoi = formData.get('matKhauMoi');
        const confirmMatKhauMoi = formData.get('confirmMatKhauMoi');

        if (matKhauMoi.length < 6) {
            msg.innerHTML = '<div class="text-danger mb-2 small fw-bold">Mật khẩu mới phải có ít nhất 6 ký tự!</div>';
            return;
        }
        if (matKhauMoi === matKhauCu) {
            msg.innerHTML = '<div class="text-danger mb-2 small fw-bold">Mật khẩu mới không được trùng mật khẩu cũ!</div>';
            return;
        }
        if (matKhauMoi !== confirmMatKhauMoi) {
            msg.innerHTML = '<div class="text-danger mb-2 small fw-bold">Mật khẩu xác nhận không khớp!</div>';
            return;
        }

        const btn = e.target.querySelector('button[type="submit"]');
        btn.disabled = true; btn.innerText = 'Đang xử lý...';

        try {
            const res = await authFetch('/api/User/doi-mat-khau', {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    matKhauCu: matKhauCu,
                    matKhauMoi: matKhauMoi
                })
            });
            if (res.ok) {
                msg.innerHTML = '<div class="text-success fw-bold mb-2">Đổi mật khẩu thành công!</div>';
                setTimeout(() => modal.hide(), 1500);
            } else {
                const data = await res.json();
                msg.innerHTML = `<div class="text-danger mb-2 small fw-bold">${data.message || 'Sai mật khẩu cũ'}</div>`;
                btn.disabled = false; btn.innerText = 'XÁC NHẬN ĐỔI';
            }
        } catch (err) { 
            msg.innerHTML = `<div class="text-danger mb-2 small fw-bold">Lỗi kết nối hoặc sai mật khẩu cũ</div>`;
            btn.disabled = false; btn.innerText = 'XÁC NHẬN ĐỔI';
        }
    };
}

if (location.pathname.includes('chinh-sach.html')) renderPolicyContent();
