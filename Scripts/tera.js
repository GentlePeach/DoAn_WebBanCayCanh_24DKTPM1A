
var currentCategoryId = null;
var currentMinPrice = null;
var currentMaxPrice = null;

// Hàm lọc Danh mục (Category)
function filterCategory(element, id) {
    console.log("Đã bấm danh mục ID: " + id); // Kiểm tra xem hàm có chạy không
    $('.list-group-item').removeClass('active');
    $(element).addClass('active');

    currentCategoryId = id;
    loadProducts();
}

// Hàm lọc Giá (Price)
function filterPrice(element) {
    console.log("Đã tích vào ô giá: " + $(element).val()); // Kiểm tra

    if ($(element).is(':checked')) {
        // Chỉ cho phép chọn 1 ô giá -> Bỏ tích các ô khác
        $('.price-checkbox').not(element).prop('checked', false);

        var value = $(element).val();
        if (value) {
            var parts = value.split('-');
            currentMinPrice = parts[0];
            currentMaxPrice = parts[1];
        }
    } else {
        // Nếu bỏ tích -> Reset về null
        currentMinPrice = null;
        currentMaxPrice = null;
    }

    loadProducts();
}

// HÀM AJAX GỬI VỀ SERVER
function loadProducts() {
    console.log("Đang gửi Ajax với dữ liệu:", {
        categoryId: currentCategoryId,
        minPrice: currentMinPrice,
        maxPrice: currentMaxPrice
    });

    $('#product-list-container').css('opacity', '0.5');

    $.ajax({
        url: '/Products/Tera', 
        type: 'GET',
        data: {
            categoryId: currentCategoryId,
            minPrice: currentMinPrice,
            maxPrice: currentMaxPrice,
            page: 1 
        },
        success: function (result) {
            console.log("Đã nhận kết quả từ Server!");
            $('#product-list-container').html(result);
            $('#product-list-container').css('opacity', '1');
        },
        error: function (xhr, status, error) {
            console.error("Lỗi Ajax:", error);
            alert("Lỗi tải dữ liệu. Vui lòng nhấn F12 -> Console để xem chi tiết.");
            $('#product-list-container').css('opacity', '1');
        }
    });
}