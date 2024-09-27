const apiUrl = 'http://localhost:5139/api/orders';  // Ensure the correct API endpoint

// Müşteri Siparişi Oluşturma
document.getElementById('orderForm')?.addEventListener('submit', async (e) => {
    e.preventDefault();

    const applicantName = document.getElementById('applicantName').value;
    const productID = document.getElementById('productID').value;
    const quantity = document.getElementById('quantity').value;
    const shippingAddress = document.getElementById('shippingAddress').value;

    const order = {
        applicantName,
        productID: parseInt(productID),
        quantity: parseInt(quantity),
        shippingAddress
    };

    try {
        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(order)
        });

        if (response.ok) {
            alert('Sipariş başarıyla oluşturuldu!');
        } else {
            alert('Sipariş oluşturulamadı. Bir hata meydana geldi.');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Sipariş oluşturulamadı. Bir hata meydana geldi.');
    }
});

// Şirket Çalışanı Sipariş Görüntüleme ve Onaylama
async function fetchOrders() {
    try {
        const response = await fetch('http://localhost:5139/api/orders');
                const orders = await response.json();
                console.log(orders);  
          
        

        const orderList = document.getElementById('orderList');
        orderList.innerHTML = '';  // Clear previous list

        orders.forEach(order => {
           

            const listItem = document.createElement('li');
            listItem.textContent = `Müşteri: ${order.applicantName}, Ürün: ${order.productID}, Miktar: ${order.quantity}, Fiyat: ${order.price} TL, Adres: ${order.shippingAddress}, Onay Durumu: ${order.isApproved ? 'Onaylandı' : 'Beklemede'}`;

            if (!order.isApproved) {
                // Onayla butonu
                const confirmButton = document.createElement('button');
                confirmButton.textContent = 'Onayla';
                confirmButton.addEventListener('click', async () => {
                    await approveOrder(order.applicationID);  // Notice: correct casing
                    listItem.textContent = `Müşteri: ${order.applicantName}, Ürün: ${order.productID}, Miktar: ${order.quantity}, Fiyat: ${order.price} TL, Onay Durumu: Onaylandı`;
                    confirmButton.remove();
                    rejectButton.remove();
                });
                listItem.appendChild(confirmButton);

                // Reddet butonu
                const rejectButton = document.createElement('button');
                rejectButton.textContent = 'Reddet';
                rejectButton.style.backgroundColor = 'red';
                rejectButton.style.color = 'white';
                rejectButton.style.marginLeft = '10px';
                rejectButton.addEventListener('click', async () => {
                    await rejectOrder(order.applicationID);  // Notice: correct casing
                    listItem.textContent = `Müşteri: ${order.applicantName}, Ürün: ${order.productID}, Miktar: ${order.quantity}, Fiyat: ${order.price} TL, Onay Durumu: Reddedildi`;
                    confirmButton.remove();
                    rejectButton.remove();
                });
                listItem.appendChild(rejectButton);
            }

            orderList.appendChild(listItem);
        });
    } catch (error) {
        console.error('Error fetching orders:', error);
        alert('Siparişler alınamadı.');
    }
}

// PUT isteği sırasında hata yakalamak için
async function approveOrder(orderId) {
    try {
        const response = await fetch(`http://localhost:5139/api/orders/${orderId}/confirm`, {
            method: 'PUT',
        });

        if (response.ok) {
            alert('Sipariş onaylandı!');
            fetchOrders();  // Siparişler güncellenir
        } else {
            alert('Siparişi onaylama başarısız oldu.');
            console.log(await response.text());  // Hata mesajını logla
        }
    } catch (error) {
        console.error('Error on approving order:', error);
    }
}

async function rejectOrder(orderId) {
    try {
        const response = await fetch(`http://localhost:5139/api/orders/${orderId}/reject`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        

        if (response.ok) {
            alert('Sipariş reddedildi!');
            fetchOrders();  // Siparişler güncellenir
        } else {
            alert('Siparişi reddetme başarısız oldu.');
            console.log(await response.text());  // Hata mesajını logla
        }
    } catch (error) {
        console.error('Error on rejecting order:', error);
    }
}



// Sayfa yüklendiğinde siparişleri getir
fetchOrders();
