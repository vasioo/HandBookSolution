$(document).ready(function () {
    const $profileImages = $('.image-upload');

    // Event listener for file input change
    $profileImages.change(function (event) {
        const $self = $(this),
            $uploadedImage = $self.closest('.uploaded-image'),
            $uploadedImageContainer = $('#custom-view-profile-image-id'),
            file = event.target.files[0];

        validateAndResizeImage(file, function (isValid, imageData) {
            if (isValid) {
                $uploadedImage.attr('src', imageData);
                $uploadedImageContainer.attr('src', imageData);

            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Image validation failed:' + imageData
                })
            }
        });
    });
});
function validateAndResizeImage(file, callback) {

    const maxSizeInPixels = 1024, // Maximum dimension in pixels
        maxSizeInBytes = 1 * 1024 * 1024; // 1MB in bytes

    if (file.type.includes('image/')) {
        const reader = new FileReader();
        reader.onload = function (e) {
            const image = new Image();
            image.onload = function () {
                if (file.size <= maxSizeInBytes && (image.width > maxSizeInPixels || image.height > maxSizeInPixels)) {
                    resizeImage(image, maxSizeInPixels, function (resizedDataUrl) {
                        if (getImageSizeInBytes(resizedDataUrl) <= maxSizeInBytes) {
                            callback(true, resizedDataUrl);
                        } else {
                            callback(false, 'Resized image size exceeds the maximum limit of 5MB.');
                        }
                    });
                } else {
                    callback(true, e.target.result);
                }
            };
            image.src = e.target.result;
        };
        reader.readAsDataURL(file);
    } else {
        callback(false, 'Invalid file type.');
    }
}
function resizeImage(image, maxSize, callback) {
    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');

    let width = image.width;
    let height = image.height;

    if (width > height) {
        if (width > maxSize) {
            height *= maxSize / width;
            width = maxSize;
        }
    } else {
        if (height > maxSize) {
            width *= maxSize / height;
            height = maxSize;
        }
    }

    canvas.width = width;
    canvas.height = height;

    ctx.drawImage(image, 0, 0, width, height);

    const resizedDataUrl = canvas.toDataURL('image/jpeg'); // Adjust the format as needed

    callback(resizedDataUrl);
}
function getImageSizeInBytes(dataUrl) {
    const base64String = dataUrl.split(',')[1],
        padding = (base64String.length % 4 === 0 ? 0 : 4 - (base64String.length % 4)),
        base64 = base64String + '='.repeat(padding),
        sizeInBytes = (base64.length * 0.75) - (padding);

    return sizeInBytes;
}
$('.add-following').click(function () {
    var buttonElement = $(this);
    var username = buttonElement.data("username");

    $.ajax({
        type: "POST",
        url: "/Home/AddFollowerRelationship",
        data: { username: username },
        success: function (data) {
            buttonElement.removeClass("btn-primary add-following").addClass("btn-secondary remove-following");
            buttonElement.text("Following");
        },
        error: function (error) {
            console.log("Error loading more posts: " + error.responseText);
        }
    });
});
$('.remove-following').click(function () {
    var buttonElement = $(this);
    var username = buttonElement.data("username");

    $.ajax({
        type: "POST",
        url: "/Home/RemoveFollowerRelationship",
        data: { username: username },
        success: function (data) {
            buttonElement.removeClass("btn-secondary remove-following").addClass("btn-primary add-following");
            buttonElement.text("Follow");
        },
        error: function (error) {
            console.log("Error loading more posts: " + error.responseText);
        }
    });
});

function showProfileImage() {
    var itemContainer = document.getElementById('custom-view-profile-image-container-id');
    itemContainer.style.setProperty('display', 'flex', 'important');

    var neededBody = document.getElementsByTagName('body')[0];
    neededBody.style.overflow = 'hidden';
}


$('#custom-view-profile-image-container-id').click(function () {
    var itemContainer = document.getElementById('custom-view-profile-image-container-id');
    itemContainer.style.setProperty('display', 'none', 'important');

    var neededBody = document.getElementsByTagName('body')[0];
    neededBody.style.overflow = 'hidden';
});