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

$('#follows-load-btn').click(function () {
    getFollows(true, $(this).data('offset-count'));
});

$('#followers-load-btn').click(function () {
    getFollows(true, $(this).data('offset-count'));
});

function getFollows(isFollowers, offsetItem) {

    $.ajax({
        type: "POST",
        url: "/Home/LoadFollows",
        data: { followers: isFollowers, offset: offsetItem },
        success: function (data) {
            var follows = data.message;
            var fragment = document.createDocumentFragment();
            var indexer = 0;
            follows.forEach(function () {
                var imageLink = `https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-${follows[indexer]}.png`;
                var btnType = isFollowers ? `<div class="btn btn-secondary remove-follower"  id="${follows[indexer]}">Remove</div>` : `<div class="btn btn-secondary remove-following-overlay" id="${follows[indexer]}">Following</div>`;
                var followerDivContainer = `
                    <div class="col-2" >
                        <a class=" user-username-link" href="Account?username=${follows[indexer]}">
                            <img src="${imageLink}" style="width:40px;height:40px;border-radius:30px;">
                        </a>
                    </div >
                    <div class="col-6">
                        <a class=" user-username-link" href="Account?username=${follows[indexer]}">
                            <div class="row">@${follows[indexer]}</div>
                        </a>
                    </div>
                    <div class="col-4 text-center">
                        ${btnType}
                    </div>
                </div >
                `;
                var tempDiv = document.createElement('div');
                tempDiv.setAttribute('class', 'row');
                tempDiv.innerHTML = followerDivContainer;
                fragment.appendChild(tempDiv);
                indexer++;
            });

            const existingOverlay = document.getElementById('overlay-panel-for-follows');

            if (existingOverlay) {
                existingOverlay.remove();

                document.body.style.overflow = 'auto';
                $('.comment-section-regulation-div').empty();

                document.removeEventListener('click', handleOutsideClickForFollowers);
            }
            const overlay = document.createElement('div');
            overlay.setAttribute('class', 'd-flex justify-content-center align-items-center');
            overlay.id = "overlay-panel-for-follows";
            document.body.appendChild(overlay);
            document.body.style.overflow = 'hidden';

            const overlayContent = document.createElement('div');
            overlayContent.setAttribute('class', 'overlay-content');
            overlayContent.append(fragment);
            overlay.appendChild(overlayContent);


            function handleOutsideClickForFollowers(event) {
                if (event.target === overlay) {
                    document.body.style.overflow = 'auto';
                    overlay.remove();
                    document.removeEventListener('click', handleOutsideClickForFollowers);
                }
            }

            document.addEventListener('click', handleOutsideClickForFollowers);

            if (isFollowers) {
                $('#followers-load-btn').data('offset-count', offsetItem + 1);
            }
            else {
                $('#follows-load-btn').data('offset-count', offsetItem + 1);
            }
            var $overlayPanel = $('#overlay-panel-for-follows');
            $overlayPanel.css('display', 'block');

            $overlayPanel.append(fragment);
        },
        error: function (error) {
            console.log("Error loading more comments: " + error.responseText);
            loading = false;
        }
    });
}

$(document).on('click', '.remove-follower', function () {
    var usernametemp = $(this).attr('id');
    Swal.fire({
        title: 'Confirmation',
        text: `Are you sure you want to remove ${usernametemp} as a follower?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, remove!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Home/RemoveFollowerRelationship",
                data: { username: usernametemp },
                success: function (data) {
                    $(this).removeClass("btn-secondary remove-follower").addClass("btn-primary add-follower");
                    $(this).text("Unfollow");
                },
                error: function (error) {
                    console.log("Error loading more posts: " + error.responseText);
                }
            });
        }
    });
});

$(document).on('click', '.remove-following-overlay', function () {
    var usernametemp = $(this).attr('id');
    Swal.fire({
        title: 'Confirmation',
        text: `Are you sure you want to remove ${usernametemp} as a follower?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, remove!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Home/RemoveFollowerRelationship",
                data: { username: usernametemp, shouldBeReversed: true },
                success: function (data) {
                    $(this).closest('.user-username-link').remove();
                },
                error: function (error) {
                    console.log("Error loading more posts: " + error.responseText);
                }
            });
        }
    });
});

$(document).on('click', '.add-follower', function () {
    var usernametemp = $(this).attr('id');
    $.ajax({
        type: "POST",
        url: "/Home/AddFollowerRelationship",
        data: { username: usernametemp },
        success: function (data) {
            $(this).removeClass("btn-secondary add-follower").addClass("btn-primary remove-follower")
            $(this).text("Follow");
        },
        error: function (error) {
            console.log("Error loading more posts: " + error.responseText);
        }
    });
});