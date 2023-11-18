$(document).on('click', '.submit-btn', function () {
    var $commentContainer = $(this).closest('.comment-section');
    var $commentTextarea = $commentContainer.find('.my-text');

    var commentContent = $commentTextarea.val();
    var postId = $commentContainer.data('post-id');

    var comment = {
        CommentContent: commentContent,
        DateOfCreation: new Date().toString(),
        PostId: postId
    };

    $.ajax({
        url: "/Home/AddOrRemoveAComment",
        method: 'POST',
        data: { commentsDTO: comment },
        dataType: 'json',
        success: function (response) {
            var commentDto = JSON.parse(response);

            var usernameLink = 'https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-' + commentDto.UserUsername + '.png';
            var alternativeLink = 'https://res.cloudinary.com/dzaicqbce/image/upload/v1700160046/azgysbpf8xpcxpfclb9i.jpg';
            var imgTag = '<img src="' + usernameLink + '" alt="Image not found" onerror="this.onerror=null;this.src=\'' + alternativeLink + '\';" style="border-radius:30px; width:50px; margin-right:10px;" />';

            var commentHtml = `
            <div class="comment-container pt-2">
                <div class="profile-column">
                   `+ imgTag + `
                </div>
                <div class="comment-column">
                    <div class="comment">
                        <div class="comment-header">
                            <div class="comment-username">${commentDto.UserUsername}</div>
                            <div class="comment-time">Now</div>
                        </div>
                        <div class="comment-content">${commentDto.CommentContent}</div>
                        <div class="comment-actions">
                            <a href="#" class="comment-action">Like</a>
                            <a href="#" class="comment-action">Reply</a>
                        </div>
                    </div>
                    <div class="replies-container">
                        <!-- Add replies here if available -->
                    </div>
                    <div class="view-replies">View Replies</div>
                </div>
            </div>
        `;

            $('.comment-section-regulation-div').prepend(commentHtml);


        },
        error: function (error) {
            // Add logging
            console.error('Error submitting comment:', error);
        }
    });
});


function likeButtonClick(Id) {
    $.ajax({
        type: "POST",
        url: "/Home/IncrementOrDecrementLikeCount",
        data: { itemId: Id },
        success: function (result) {
            var likeCountElement = $('.likeCount[data-item-id="' + Id + '"]');
            likeCountElement.html(result + ' <i class="fa-solid fa-heart liked fa-sm" style="color: #ff0000;"></i>');
            likeCountElement.toggle(result > 0);
        },
        error: function (result) {
            alert("Error: " + result.statusText);
        }
    });
}

function toggleComments(itemId, element) {
    const card = element.closest('.card');
    const cardId = `card-overlay-${itemId}`;

    const existingOverlay = document.getElementById(cardId);

    if (existingOverlay) {
        existingOverlay.remove();
        const commentSection = card.querySelector('.comment-section');
        commentSection.style.display = 'none';

        card.style.zIndex = '';
        card.style.position = ''; // Reset card position

        document.removeEventListener('click', handleClickOutside);
        return;
    }

    const overlay = document.createElement('div');
    overlay.id = cardId;

    function handleClickOutside(event) {
        if (event.target === overlay) {
            card.style.zIndex = '';
            card.style.position = ''; // Reset card position
            overlay.remove();

            const commentSection = card.querySelector('.comment-section');
            commentSection.style.display = 'none';

            document.removeEventListener('click', handleClickOutside);
        }
    }

    card.style.zIndex = 1000;
    card.style.position = 'relative'; // Adjust card position to maintain its layout in the document flow

    overlay.style.position = 'fixed';
    overlay.style.top = '0';
    overlay.style.left = '0';
    overlay.style.width = '100%';
    overlay.style.height = '100%';
    overlay.style.backgroundColor = 'rgba(0, 0, 0, 0.7)';
    overlay.style.backdropFilter = 'blur(5px)'; // Apply a blur effect to the backdrop

    const commentSection = card.querySelector('.comment-section');

    if (commentSection.style.display === 'none' || commentSection.style.display === '') {
        commentSection.style.display = 'block';

        // Scroll to the top of the card
        card.scrollIntoView({ behavior: 'smooth', block: 'start', inline: 'nearest' });
    } else {
        commentSection.style.display = 'none';
    }

    document.body.appendChild(overlay);

    document.addEventListener('click', handleClickOutside);
}


function like(button, Id) {
    var card = button.closest(".card");
    var cardId = card.dataset.id;
    var count = parseInt(button.dataset.count) || 0;
    var likedCards = JSON.parse(sessionStorage.getItem("likedCards")) || {};

    var heartIcon = button.querySelector("i.fa-heart");


    if (heartIcon.classList.contains("liked")) {
        count--;
        button.dataset.count = count;
        button.innerHTML = ` <i class="fa-regular fa-heart fa-xl" style="color: #000;"></i>`;
        likeButtonClick(Id);
        delete likedCards[cardId];
    } else {
        count++;
        button.dataset.count = count;
        button.innerHTML = ` <i class="fa-solid fa-heart fa-xl liked" style="color: #ff0000;"></i>`;
        likeButtonClick(Id);
        likedCards[cardId] = true;
    }

    sessionStorage.setItem("likedCards", JSON.stringify(likedCards));
}