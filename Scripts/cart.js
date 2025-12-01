document.addEventListener('DOMContentLoaded', function() {
        const updateLocalUI = (row, data) => {
            if (data.itemTotal !== undefined) {
                row.querySelector('.item-total').textContent = (data.itemTotal).toLocaleString('vi-VN') + '₫';
            }
            if (data.subtotal !== undefined) {
                document.getElementById('cart-subtotal').textContent = (data.subtotal).toLocaleString('vi-VN') + '₫';
                document.getElementById('cart-total').textContent = (data.subtotal).toLocaleString('vi-VN') + '₫';
            }
        };

        // Plus
        document.querySelectorAll('.btn-plus').forEach(btn => {
            btn.addEventListener('click', function() {
                const id = this.dataset.id;
                const row = this.closest('.cart-item');
                const input = row.querySelector('.quantity-input');
                let qty = parseInt(input.value) +1;

                fetch('/Home/UpdateQuantity', {
                    method: 'POST',
                    headers: {'Content-Type':'application/json'},
                    body: JSON.stringify({ id: parseInt(id), qty: qty })
                }).then(r => r.json()).then(data => {
                    if (data.success) {
                        input.value = qty;
                        updateLocalUI(row, data);
                    }
                });
            });
        });

        // Minus
        document.querySelectorAll('.btn-minus').forEach(btn => {
            btn.addEventListener('click', function() {
                const id = this.dataset.id;
                const row = this.closest('.cart-item');
                const input = row.querySelector('.quantity-input');
                let qty = parseInt(input.value) -1;
                if (qty <1) return;

                fetch('/Home/UpdateQuantity', {
                    method: 'POST',
                    headers: {'Content-Type':'application/json'},
                    body: JSON.stringify({ id: parseInt(id), qty: qty })
                }).then(r => r.json()).then(data => {
                    if (data.success) {
                        input.value = qty;
                        updateLocalUI(row, data);
                    }
                });
            });
        });

        // Remove
        document.querySelectorAll('.remove-item').forEach(btn => {
            btn.addEventListener('click', function(e) {
                e.preventDefault();
                const id = this.dataset.id;
                const row = this.closest('.cart-item');
                if (!confirm('Bạn muốn xóa sản phẩm này?')) return;

                fetch('/Home/RemoveFromCart', {
                    method: 'POST',
                    headers: {'Content-Type':'application/json'},
                    body: JSON.stringify({ id: parseInt(id) })
                }).then(r => r.json()).then(data => {
                    if (data.success) {
                        row.remove();
                        document.getElementById('cart-subtotal').textContent = (data.subtotal).toLocaleString('vi-VN') + '₫';
                        document.getElementById('cart-total').textContent = (data.subtotal).toLocaleString('vi-VN') + '₫';
                    }
                });
            });
        });

    });
