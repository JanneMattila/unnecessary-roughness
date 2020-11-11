var _imagesLoaded = 0;
var _imagesToLoad = -1;
var _images = [];
var _IMAGE_END_ZONE = 0;
var _IMAGE_FLOOR_LIGHT = 2;
var _IMAGE_BALL = 3;
var _IMAGE_BALL_ANIMATION = 4;
var _IMAGE_PLAYER1 = 5;
var _FLOOR_SIZE = 35;
function loadImages() {
    var theme = "basic";
    var files = [
        "player-32x32.png"
    ];
    _imagesToLoad = files.length;
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var img = new Image();
        img.onload = function () {
            _imagesLoaded++;
        };
        img.src = "/images/themes/" + theme + "/" + file;
        _images[i] = img;
    }
}
loadImages();
function drawPlayer(context, player, imageIndex, x, y, rotation, selectedPlayerX, selectedPlayerY) {
    if (_animationPlayers !== undefined && _animationPlayers.length > 0) {
        for (var i = 0; i < _animationPlayers.length; i++) {
            var animationPlayer = _animationPlayers[i];
            if (animationPlayer === null) {
                // Animation of this player has already finished.
                continue;
            }
            if (animationPlayer.id === player.id) {
                // This player is under animation.
                context.save();
                context.translate(animationPlayer.position.x + _FLOOR_SIZE / 2, animationPlayer.position.y + _FLOOR_SIZE / 2);
                context.rotate(Math.PI * animationPlayer.rotation / 180);
                context.translate(-_FLOOR_SIZE / 2, -_FLOOR_SIZE / 2);
                context.drawImage(_images[imageIndex], 0, 0);
                context.restore();
                return true;
            }
        }
    }
    if (x === selectedPlayerX && y === selectedPlayerY) {
        // Green
        context.save();
        context.fillStyle = "rgba(144,238,144, 0.3)";
        context.beginPath();
        context.fillRect(x * _FLOOR_SIZE, y * _FLOOR_SIZE, _FLOOR_SIZE, _FLOOR_SIZE);
        context.fill();
        context.restore();
    }
    if (player.knockDown) {
        context.save();
        context.translate(x * _FLOOR_SIZE, y * _FLOOR_SIZE);
        context.drawImage(_images[imageIndex + 1], 0, 0);
        context.restore();
    }
    else {
        context.save();
        context.translate(x * _FLOOR_SIZE + _FLOOR_SIZE / 2, y * _FLOOR_SIZE + _FLOOR_SIZE / 2);
        context.rotate(Math.PI * rotation / 180);
        context.translate(-_FLOOR_SIZE / 2, -_FLOOR_SIZE / 2);
        context.drawImage(_images[imageIndex], 0, 0);
        context.restore();
    }
    return false;
}
var _scrollElements = new Object();
function showElement(id, modal) {
    var element = document.getElementById(id);
    //let clientX = e.clientX || e.touches[0].clientX;
    //let clientY = e.clientY || e.touches[0].clientY;
    //offsetX = startX - clientX;
    //offsetY = startY - clientY;
    //startX = clientX;
    //startY = clientY;
    element.style.display = "";
    var y = window.scrollY;
    if (modal) {
        var width = _canvasElement != null ? _canvasElement.width : 900;
        var x = width / 2 - element.offsetWidth / 2;
        console.log("x: " + x);
        element.style.left = x + "px";
        y += window.innerHeight / 2 - element.offsetHeight / 2;
    }
    element.style.top = y + "px";
    _scrollElements[id] = modal;
}
function scrollElement() {
    for (var id in _scrollElements) {
        var modal = _scrollElements[id];
        var element = document.getElementById(id);
        var y = window.scrollY;
        if (modal) {
            y += window.innerHeight / 2 - element.offsetHeight / 2;
        }
        element.style.top = (y) + "px";
    }
}
function hideElement(id) {
    var element = document.getElementById(id);
    element.style.display = "none";
    delete _scrollElements[id];
}
function initializeMovableElements() {
    var dialogs = document.getElementsByClassName("dialogdiv");
    for (var i = 0; i < dialogs.length; i++) {
        createMovableElement(dialogs[i]);
    }
    function createMovableElement(element) {
        var dialogTitleElement = element.firstElementChild;
        var startX = 0, startY = 0, offsetX = 0, offsetY = 0;
        dialogTitleElement.addEventListener("mousedown", startElementMove, false);
        dialogTitleElement.addEventListener("touchstart", startElementMove, false);
        function startElementMove(e) {
            e.preventDefault();
            startX = e.clientX || e.touches[0].clientX;
            startY = e.clientY || e.touches[0].clientY;
            document.addEventListener("mouseup", endElementMove, false);
            document.addEventListener("mousemove", elementMove, false);
            document.addEventListener("touchend", endElementMove, false);
            document.addEventListener("touchmove", elementMove, false);
        }
        function elementMove(e) {
            e.preventDefault();
            var clientX = e.clientX || e.touches[0].clientX;
            var clientY = e.clientY || e.touches[0].clientY;
            offsetX = startX - clientX;
            offsetY = startY - clientY;
            startX = clientX;
            startY = clientY;
            element.style.top = (element.offsetTop - offsetY) + "px";
            element.style.left = (element.offsetLeft - offsetX) + "px";
        }
        function endElementMove() {
            document.removeEventListener("mouseup", endElementMove, false);
            document.removeEventListener("touchend", endElementMove, false);
            document.removeEventListener("mousemove", elementMove, false);
            document.removeEventListener("touchmove", elementMove, false);
        }
    }
}
var _canvasElement;
var _context;
function initializeCanvas(canvasElement) {
    _canvasElement = canvasElement;
    _context = _canvasElement.getContext("2d");
    _context.font = "14pt Arial";
    _canvasElement.addEventListener("click", function (event) {
        calculatePosition(event);
    });
}
function initializeGameview(canvasElement) {
    initializeCanvas(canvasElement);
    initializeMovableElements();
}
var _animationRunning = false;
var _animationUpdate = 0;
var _animationPlayers;
var _animationBall;
var _game;
var BoardPosition = /** @class */ (function () {
    function BoardPosition() {
    }
    return BoardPosition;
}());
var AnimationMove = /** @class */ (function () {
    function AnimationMove() {
    }
    return AnimationMove;
}());
var ObjectAnimation = /** @class */ (function () {
    function ObjectAnimation() {
    }
    return ObjectAnimation;
}());
function update(timestamp) {
    var delta = (timestamp - _animationUpdate) / 1000;
    if (_animationBall != null) {
        if (_animationBall.startTime > 0) {
            _animationBall.startTime -= delta;
        }
        if (_animationBall.startTime <= 0) {
            var deltaBall = delta + _animationBall.startTime;
            _animationBall.startTime = 0;
            var currentAnimation = _animationBall.animations[_animationBall.index];
            var dx = (currentAnimation.target.x - currentAnimation.source.x) * _FLOOR_SIZE;
            var dy = (currentAnimation.target.y - currentAnimation.source.y) * _FLOOR_SIZE;
            if (currentAnimation.current == null) {
                console.log("Ball started");
                currentAnimation.current = new BoardPosition();
                currentAnimation.current.x = currentAnimation.source.x * _FLOOR_SIZE;
                currentAnimation.current.y = currentAnimation.source.y * _FLOOR_SIZE;
            }
            else {
                currentAnimation.timeElapsed += deltaBall;
                if (currentAnimation.timeElapsed >= currentAnimation.animationTime) {
                    currentAnimation.timeElapsed = currentAnimation.animationTime;
                    console.log("Ball animation next animation.");
                    _animationBall.index++;
                    if (_animationBall.index >= _animationBall.animations.length) {
                        console.log("Ball animation finished.");
                        _animationBall = null;
                    }
                }
            }
            if (_animationBall != null) {
                var percentage = currentAnimation.timeElapsed / currentAnimation.animationTime;
                currentAnimation.current.x = currentAnimation.source.x * _FLOOR_SIZE + dx * percentage;
                currentAnimation.current.y = currentAnimation.source.y * _FLOOR_SIZE + dy * percentage;
                if (_animationBall.rotation != -1) {
                    _animationBall.rotation += deltaBall;
                }
                _animationBall.position = currentAnimation.current;
            }
        }
    }
    if (_animationPlayers != null) {
        var playerAnimationsStillRunning = false;
        for (var animationPlayerIndex = 0; animationPlayerIndex < _animationPlayers.length; animationPlayerIndex++) {
            var animationPlayer = _animationPlayers[animationPlayerIndex];
            if (animationPlayer == null) {
                // This player animation has already finished.
                continue;
            }
            playerAnimationsStillRunning = true;
            var currentAnimation = animationPlayer.animations[animationPlayer.index];
            var dx = (currentAnimation.target.x - currentAnimation.source.x) * _FLOOR_SIZE;
            var dy = (currentAnimation.target.y - currentAnimation.source.y) * _FLOOR_SIZE;
            if (currentAnimation.current == null) {
                currentAnimation.current = new BoardPosition();
                currentAnimation.current.x = currentAnimation.source.x * _FLOOR_SIZE;
                currentAnimation.current.y = currentAnimation.source.y * _FLOOR_SIZE;
            }
            else {
                currentAnimation.timeElapsed += delta;
                if (currentAnimation.timeElapsed >= currentAnimation.animationTime) {
                    currentAnimation.timeElapsed = currentAnimation.animationTime;
                    console.log("Player animation next animation.");
                    animationPlayer.index++;
                    if (animationPlayer.index >= animationPlayer.animations.length) {
                        console.log("Player animation finished.");
                        animationPlayer = null;
                        _animationPlayers[animationPlayerIndex] = null;
                    }
                }
            }
            if (animationPlayer != null) {
                var percentage = currentAnimation.timeElapsed / currentAnimation.animationTime;
                currentAnimation.current.x = currentAnimation.source.x * _FLOOR_SIZE + dx * percentage;
                currentAnimation.current.y = currentAnimation.source.y * _FLOOR_SIZE + dy * percentage;
                animationPlayer.rotation = currentAnimation.rotation;
                animationPlayer.position = currentAnimation.current;
            }
        }
        if (!playerAnimationsStillRunning) {
            _animationPlayers = null;
        }
    }
}
function gameViewAnimationFrame(timestamp) {
    if (_animationUpdate == 0) {
        _animationUpdate = timestamp;
    }
    update(timestamp);
    drawCanvas(_game);
    if ((_animationPlayers != null && _animationPlayers.length > 0) ||
        _animationBall != null) {
        window.requestAnimationFrame(gameViewAnimationFrame);
    }
    else {
        // Animation has ended.
        console.log("AnimationEnded");
        DotNet.invokeMethod("UR.Client", "AnimationEnded");
    }
}
function animate(animationPlayers, animationBall) {
    console.log("animate:");
    console.log(animationPlayers);
    console.log(animationBall);
    _animationUpdate = 0;
    _animationPlayers = animationPlayers;
    _animationBall = animationBall;
    gameViewAnimationFrame(0);
}
function drawCanvas(game) {
    _game = game;
    // TODO: Analyze that do we get a lot of these draw commands.
    //console.log(_game);
    if (_context === null) {
        return;
    }
    _context.save();
    _context.clearRect(0, 0, _canvasElement.width, _canvasElement.height);
    var width = 25;
    var height = 50;
    var xDrawOffset = 1;
    var yDrawOffset = 1;
    for (var x = 0; x <= width; x++) {
        // North-south lines
        _context.beginPath();
        _context.moveTo(x * _FLOOR_SIZE + xDrawOffset, yDrawOffset);
        _context.lineTo(x * _FLOOR_SIZE + xDrawOffset, height * _FLOOR_SIZE + yDrawOffset);
        _context.stroke();
    }
    for (var y = 0; y <= height; y++) {
        // East-West lines
        _context.beginPath();
        _context.moveTo(xDrawOffset, y * _FLOOR_SIZE + yDrawOffset);
        _context.lineTo(width * _FLOOR_SIZE + xDrawOffset, y * _FLOOR_SIZE + yDrawOffset);
        _context.stroke();
    }
    if (_imagesLoaded === _imagesToLoad) {
        //console.log("Draw board");
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                var availableMovementSelection = false;
                var availableExtraMovementSelection = false;
                var selectedMovementSelection = false;
                for (var m = 0; m < game.availableMoves.length; m++) {
                    var move = game.availableMoves[m];
                    if (move.x === x && move.y === y) {
                        availableMovementSelection = true;
                        break;
                    }
                }
                for (var m = 0; m < game.availableExtraMoves.length; m++) {
                    var move = game.availableExtraMoves[m];
                    if (move.x === x && move.y === y) {
                        availableExtraMovementSelection = true;
                        break;
                    }
                }
                for (var m = 0; m < game.selectedMoves.length; m++) {
                    var move = game.selectedMoves[m];
                    if (move.x === x && move.y === y) {
                        selectedMovementSelection = true;
                        break;
                    }
                }
                if (selectedMovementSelection) {
                    // Dark yellow
                    _context.save();
                    _context.fillStyle = "rgba(0,0,255, 0.2)";
                    _context.beginPath();
                    _context.fillRect(x * _FLOOR_SIZE, y * _FLOOR_SIZE, _FLOOR_SIZE, _FLOOR_SIZE);
                    _context.fill();
                    _context.restore();
                }
                else if (availableMovementSelection) {
                    // Yellow
                    _context.save();
                    _context.fillStyle = "rgba(255,255,153, 0.3)";
                    _context.beginPath();
                    _context.fillRect(x * _FLOOR_SIZE, y * _FLOOR_SIZE, _FLOOR_SIZE, _FLOOR_SIZE);
                    _context.fill();
                    _context.restore();
                }
                else if (availableExtraMovementSelection) {
                    // Red
                    _context.save();
                    _context.fillStyle = "rgba(240,128,128, 0.3)";
                    _context.beginPath();
                    _context.fillRect(x * _FLOOR_SIZE, y * _FLOOR_SIZE, _FLOOR_SIZE, _FLOOR_SIZE);
                    _context.fill();
                    _context.restore();
                }
            }
        }
        var selectedPlayerX = -1, selectedPlayerY = -1;
        var ballInPlayersHands = false;
        if (game.selectedPlayer != null) {
            selectedPlayerX = game.selectedPlayer.boardPosition.x;
            selectedPlayerY = game.selectedPlayer.boardPosition.y;
        }
        for (var p = 0; p < game.homeTeam.players.length; p++) {
            var player = game.homeTeam.players[p];
            var x = player.boardPosition.x;
            var y = player.boardPosition.y;
            if (x != -1) {
                var inAnimation = drawPlayer(_context, player, _IMAGE_PLAYER1, x, y, player.rotation, selectedPlayerX, selectedPlayerY);
                if (inAnimation == false && x == game.ball.boardPosition.x && y == game.ball.boardPosition.y) {
                    ballInPlayersHands = true;
                }
            }
        }
        for (var p = 0; p < game.visitorTeam.players.length; p++) {
            var player = game.visitorTeam.players[p];
            var x = player.boardPosition.x;
            var y = player.boardPosition.y;
            if (x != -1) {
                var inAnimation = drawPlayer(_context, player, _IMAGE_PLAYER1, x, y, player.rotation, selectedPlayerX, selectedPlayerY);
                if (inAnimation == false && x == game.ball.boardPosition.x && y == game.ball.boardPosition.y) {
                    ballInPlayersHands = true;
                }
            }
        }
        if (_animationBall != null) {
            // Ball in animation
            if (_animationBall.startTime > 0) {
                // Static ball position since player has not yet reach this board position
                _context.drawImage(_images[_IMAGE_BALL_ANIMATION], _animationBall.position.x * _FLOOR_SIZE, _animationBall.position.y * _FLOOR_SIZE);
            }
            else if (_animationBall.rotation == -1) {
                // Player carrying the ball
                _context.save();
                _context.translate(_animationBall.position.x + _FLOOR_SIZE / 2, _animationBall.position.y + _FLOOR_SIZE / 2);
                _context.translate(-_FLOOR_SIZE / 2, -_FLOOR_SIZE / 2);
                _context.drawImage(_images[_IMAGE_BALL], 0, 0);
                _context.restore();
            }
            else {
                // Ball is bouncing
                _context.save();
                _context.translate(_animationBall.position.x + _FLOOR_SIZE / 2, _animationBall.position.y + _FLOOR_SIZE / 2);
                _context.rotate(Math.PI * _animationBall.rotation / 180);
                _context.translate(-_FLOOR_SIZE / 2, -_FLOOR_SIZE / 2);
                _context.drawImage(_images[_IMAGE_BALL_ANIMATION], 0, 0);
                _context.restore();
            }
        }
        else if (game.ball.boardPosition.x != -1) {
            // Static ball position
            var ballImage = ballInPlayersHands ? _images[_IMAGE_BALL] : _images[_IMAGE_BALL_ANIMATION];
            _context.drawImage(ballImage, game.ball.boardPosition.x * _FLOOR_SIZE, game.ball.boardPosition.y * _FLOOR_SIZE);
        }
    }
    _context.restore();
}
var DotNet = /** @class */ (function () {
    function DotNet() {
    }
    return DotNet;
}());
function calculatePosition(event) {
    if (!_animationRunning) {
        var x = Math.floor(event.offsetX / _FLOOR_SIZE);
        var y = Math.floor(event.offsetY / _FLOOR_SIZE);
        console.log("Clicked canvas at " + x + " - " + y);
        console.log(_game);
        DotNet.invokeMethod("UR.Client", "CanvasClickReceived", x, y);
    }
}
document.addEventListener("keydown", function (event) {
    if (event.keyCode != 123 /* F12 */) {
        event.preventDefault();
    }
});
document.addEventListener("keyup", function (event) {
    if (event.keyCode != 123 /* F12 */) {
        event.preventDefault();
        if (!_animationRunning) {
            DotNet.invokeMethod("UR.Client", "CanvasKeyUpReceived", event.keyCode);
        }
    }
});
window.addEventListener("scroll", function (event) {
    scrollElement();
});
//# sourceMappingURL=GameView.js.map