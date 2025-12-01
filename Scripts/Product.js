
    function changeImage(element, src) {
        document.getElementById('mainImage').src = src;

        document.querySelectorAll('.thumb-box').forEach(t => t.classList.remove('active'));

    element.classList.add('active');
}

    function stepUp() {document.getElementById('qtyInput').stepUp(); }
    function stepDown() {document.getElementById('qtyInput').stepDown(); }

