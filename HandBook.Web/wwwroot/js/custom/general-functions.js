function checkImage(url) {
    return fetch(url, { method: 'HEAD' })
        .then(res => res.ok)
        .catch(error => {
            console.error('Error checking image:', error);
            return false;
        });
}

function getTimeDisplay(postDate) {
    var timeDifference = new Date() - new Date(postDate);

    var seconds = Math.floor(timeDifference / 1000);
    var minutes = Math.floor(timeDifference / (1000 * 60));
    var hours = Math.floor(timeDifference / (1000 * 60 * 60));
    var days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

    var displayString = "";

    if (days > 365 || (days > 7 && new Date(postDate).getFullYear() !== new Date().getFullYear())) {
        var options = { year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric' };
        displayString = new Date(postDate).toLocaleDateString('en-US', options);
    } else if (days > 7) {
        displayString = new Date(postDate).toLocaleDateString('en-US', { month: 'long', day: 'numeric' });
    } else if (days >= 1) {
        displayString = days === 1 ? "1 day ago" : `${Math.floor(days)} days ago`;
    } else if (hours >= 1) {
        displayString = hours === 1 ? "1 hour ago" : `${Math.floor(hours)} hours ago`;
    } else if (minutes >= 1) {
        displayString = minutes === 1 ? "1 minute ago" : `${Math.floor(minutes)} minutes ago`;
    } else {
        displayString = seconds === 1 ? "1 second ago" : `${Math.floor(seconds)} seconds ago`;
    }

    return displayString;
}

