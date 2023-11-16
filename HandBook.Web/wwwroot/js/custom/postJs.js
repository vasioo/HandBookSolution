$(document).on('click', '.submit-btn', function () {
    var $commentContainer = $(this).closest('.comment-section');
    var $commentTextarea = $commentContainer.find('.my-text');

    var commentContent = $commentTextarea.val();
    var userUsername = $commentContainer.data('user-username'); // Assuming you have user-username data attribute in comment-section
    var postId = $commentContainer.data('post-id'); // Use data-post-id directly from the comment section

    var comment = {
        AppUsername: userUsername,
        CommentContent: commentContent,
        DateOfCreation: new Date().toString(),
        PostId: postId
    };

    $.ajax({
        url: "/Home/AddOrRemoveAComment",
        method: 'POST',
        data: { commentsDTO: comment },
        success: function (response) {
            var commentHtml = `
              <div class="main-comment row">
                <div class="row">${response.appUser.userName}</div>
                <div class="row">${response.commentContent}</div>
                <div class="row">
                  <div class="col">${GetTimeDisplay(response.dateOfCreation)}</div>
                  <div class="col"><a>Like</a></div>
                  <div class="col"><a>Reply</a></div>
                </div>
              </div>
              <div class="look-replies">View Replies</div>
            `;

            $commentContainer.find('.comments-container').append(commentHtml);
            $commentTextarea.val(''); // Use val() to set the textarea value to an empty string
        },
        error: function (error) {
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
        document.body.style.overflow = ''; // Enable body scrolling

        const commentSection = card.querySelector('.comment-section');
        commentSection.style.display = 'none';

        card.style.zIndex = '';
        card.style.overflow = '';

        document.removeEventListener('click', handleClickOutside);
        return;
    }

    const overlay = document.createElement('div');
    overlay.id = cardId;

    function disableScroll() {
        document.body.style.overflow = 'hidden';
    }

    function enableScroll() {
        document.body.style.overflow = '';
    }

    function handleClickOutside(event) {
        if (event.target === overlay) {
            enableScroll();
            card.style.zIndex = '';
            card.style.overflow = '';
            overlay.remove();

            const commentSection = card.querySelector('.comment-section');
            commentSection.style.display = 'none';

            document.removeEventListener('click', handleClickOutside);
        }
    }

    card.style.zIndex = 1000;

    overlay.style.position = 'fixed';
    overlay.style.top = '0';
    overlay.style.left = '0';
    overlay.style.width = '100%';
    overlay.style.height = '100%';
    overlay.style.backgroundColor = 'rgba(0, 0, 0, 0.7)';

    const commentSection = card.querySelector('.comment-section');

    if (commentSection.style.display === 'none' || commentSection.style.display === '') {
        commentSection.style.display = 'block';

        // Scroll to the top of the card
        card.scrollIntoView({ behavior: 'smooth', block: 'start', inline: 'nearest' });

        card.style.overflow = 'auto'; // Set card overflow to make it scrollable
        disableScroll(); // Disable body scrolling
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