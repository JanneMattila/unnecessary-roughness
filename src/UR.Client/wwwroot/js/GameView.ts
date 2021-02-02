let _imagesLoaded = 0;
let _imagesToLoad = -1;
const _images = {};

const _IMAGE_BALL = 3;
const _IMAGE_BALL_ANIMATION = 4;

const _IMAGE_PLAYER1 = 0;

const _FLOOR_SIZE = 34;

let _animationRunning = false;
let _animationUpdate = 0;
let _animationPlayers: ObjectAnimation[];
let _animationBall: ObjectAnimation;
let _game: any;

const _scrollElements = new Object();

function loadImages() {
    const theme = "basic";
    const files = [
        "goalie-blue",
        "halfback-blue",
        "fullback-blue",
        "forward-blue",
        "goalie-red",
        "halfback-red",
        "fullback-red",
        "forward-red"
    ];
    _imagesToLoad = files.length;

    for (let i = 0; i < files.length; i++) {
        const file = files[i];
        const img = new Image();
        img.onload = function () {
            _imagesLoaded++;
        };
        img.src = `/images/themes/${theme}/${file}.png`;
        _images[file] = img;
    }
}

loadImages();

function drawPlayer(context, team, player, imageIndex, x, y, rotation, selectedPlayerX, selectedPlayerY) {

    const imageName = `${player.position}-${team.image}`;
    const image = _images[imageName];
    imageIndex = Math.floor(Math.random() * _imagesLoaded);
    const offsetX = 2;
    const offsetY = 2;
    if (_animationPlayers !== undefined && _animationPlayers.length > 0) {
        for (let i = 0; i < _animationPlayers.length; i++) {
            const animationPlayer = _animationPlayers[i];
            if (animationPlayer === undefined) {
                // Animation of this player has already finished.
                continue;
            }

            if (animationPlayer.id === player.id) {
                // This player is under animation.
                context.save();
                context.translate(animationPlayer.position.x + _FLOOR_SIZE / 2, animationPlayer.position.y + _FLOOR_SIZE / 2);
                context.rotate(Math.PI * animationPlayer.rotation / 180);
                context.translate(-_FLOOR_SIZE / 2 + offsetX, -_FLOOR_SIZE / 2 + offsetY);
                context.drawImage(image, 0, 0);
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
        context.translate(x * _FLOOR_SIZE + offsetX, y * _FLOOR_SIZE + offsetY);
        context.drawImage(image, 0, 0);
        context.restore();
    }
    else {
        context.save();
        context.translate(x * _FLOOR_SIZE + _FLOOR_SIZE / 2, y * _FLOOR_SIZE + _FLOOR_SIZE / 2);
        context.rotate(Math.PI * rotation / 180);
        context.translate(-_FLOOR_SIZE / 2 + offsetX, -_FLOOR_SIZE / 2 + offsetY);
        context.drawImage(image, 0, 0);
        context.restore();
    }

    return false;
}

function showElement(id: string, modal: boolean) {
    let element = document.getElementById(id);
    //let clientX = e.clientX || e.touches[0].clientX;
    //let clientY = e.clientY || e.touches[0].clientY;

    //offsetX = startX - clientX;
    //offsetY = startY - clientY;
    //startX = clientX;
    //startY = clientY;
    element.style.display = "";

    let y = window.scrollY;
    if (modal) {
        let width = _canvasElement != undefined ? _canvasElement.width : 900;
        let x = width / 2 - element.offsetWidth / 2;
        console.log("x: " + x);

        element.style.left = x + "px";

        y += window.innerHeight / 2 - element.offsetHeight / 2;
    }
    element.style.top = y + "px";
    _scrollElements[id] = modal;
}

function scrollElement() {
    for (let id in _scrollElements) {
        let modal: boolean = _scrollElements[id];
        let element = document.getElementById(id);
        let y = window.scrollY;
        if (modal) {
            y += window.innerHeight / 2 - element.offsetHeight / 2;
        }
        element.style.top = (y) + "px";
    }
}

function hideElement(id: string) {
    const element = document.getElementById(id);
    element.style.display = "none";
    delete _scrollElements[id];
}

function initializeMovableElements() {
    const dialogs = document.getElementsByClassName("dialogdiv");
    for (let i = 0; i < dialogs.length; i++) {
        createMovableElement(<HTMLElement>dialogs[i]);
    }

    function createMovableElement(element: HTMLElement) {
        const dialogTitleElement = element.firstElementChild;
        let startX = 0, startY = 0, offsetX = 0, offsetY = 0;

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

            const clientX = e.clientX || e.touches[0].clientX;
            const clientY = e.clientY || e.touches[0].clientY;

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

let _canvasElement: HTMLCanvasElement;
let _context: CanvasRenderingContext2D;

function initializeCanvas(canvasElement: HTMLCanvasElement) {
    _canvasElement = canvasElement;
    _context = _canvasElement.getContext("2d");
    _context.font = "14pt Arial";

    _canvasElement.addEventListener("click", (event: MouseEvent) => {
        calculatePosition(event);
    });
}

function initializeGameview(canvasElement: HTMLCanvasElement) {
    initializeCanvas(canvasElement);
    initializeMovableElements();
}

class BoardPosition {
    x: number;
    y: number;
}

class AnimationMove {
    source: BoardPosition;
    target: BoardPosition;
    current: BoardPosition;

    rotation: number;
    timeElapsed: number;
    animationTime: number;
}

class ObjectAnimation {
    id: string;
    index: number;

    position: BoardPosition;
    rotation: number;
    startTime: number;
    animations: AnimationMove[];
}

function update(timestamp: number) {
    const delta = (timestamp - _animationUpdate) / 1000;

    if (_animationBall !== undefined) {
        if (_animationBall.startTime > 0) {
            _animationBall.startTime -= delta;
        }

        if (_animationBall.startTime <= 0) {

            const deltaBall = delta + _animationBall.startTime;
            _animationBall.startTime = 0;

            const currentAnimation = _animationBall.animations[_animationBall.index];
            const dx = (currentAnimation.target.x - currentAnimation.source.x) * _FLOOR_SIZE;
            const dy = (currentAnimation.target.y - currentAnimation.source.y) * _FLOOR_SIZE;

            if (currentAnimation.current === undefined) {
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
                        _animationBall = undefined;
                    }
                }
            }

            if (_animationBall != undefined) {
                const percentage = currentAnimation.timeElapsed / currentAnimation.animationTime;
                currentAnimation.current.x = currentAnimation.source.x * _FLOOR_SIZE + dx * percentage;
                currentAnimation.current.y = currentAnimation.source.y * _FLOOR_SIZE + dy * percentage;

                if (_animationBall.rotation != -1) {
                    _animationBall.rotation += deltaBall;
                }
                _animationBall.position = currentAnimation.current;
            }
        }
    }

    if (_animationPlayers !== undefined) {
        let playerAnimationsStillRunning = false;
        for (let animationPlayerIndex = 0; animationPlayerIndex < _animationPlayers.length; animationPlayerIndex++) {
            let animationPlayer = _animationPlayers[animationPlayerIndex];
            if (animationPlayer === undefined) {
                // This player animation has already finished.
                continue;
            }

            playerAnimationsStillRunning = true;
            const currentAnimation = animationPlayer.animations[animationPlayer.index];
            const dx = (currentAnimation.target.x - currentAnimation.source.x) * _FLOOR_SIZE;
            const dy = (currentAnimation.target.y - currentAnimation.source.y) * _FLOOR_SIZE;

            if (currentAnimation.current === undefined) {
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
                        animationPlayer = undefined;
                        _animationPlayers[animationPlayerIndex] = undefined;
                    }
                }
            }

            if (animationPlayer !== undefined) {
                const percentage = currentAnimation.timeElapsed / currentAnimation.animationTime;
                currentAnimation.current.x = currentAnimation.source.x * _FLOOR_SIZE + dx * percentage;
                currentAnimation.current.y = currentAnimation.source.y * _FLOOR_SIZE + dy * percentage;

                animationPlayer.rotation = currentAnimation.rotation;
                animationPlayer.position = currentAnimation.current;
            }
        }

        if (!playerAnimationsStillRunning) {
            _animationPlayers = undefined;
        }
    }
}

function gameViewAnimationFrame(timestamp: number) {

    if (_animationUpdate === 0) {
        _animationUpdate = timestamp;
    }

    update(timestamp);
    drawCanvas(_game);

    if ((_animationPlayers !== undefined && _animationPlayers.length > 0) ||
        _animationBall !== undefined) {
        window.requestAnimationFrame(gameViewAnimationFrame);
    }
    else {
        // Animation has ended.
        console.log("AnimationEnded");
        DotNet.invokeMethod("UR.Client", "AnimationEnded");
    }
}

function animate(animationPlayers: ObjectAnimation[], animationBall: ObjectAnimation): void {

    console.log("animate:");
    console.log(animationPlayers);
    console.log(animationBall);

    _animationUpdate = 0;
    _animationPlayers = animationPlayers;
    _animationBall = animationBall;
    gameViewAnimationFrame(0);
}

function drawCanvas(game: any) {

    _game = game;
    // TODO: Analyze that do we get a lot of these draw commands.
    //console.log(_game);

    if (_context === undefined) {
        return;
    }

    _context.save();
    _context.clearRect(0, 0, _canvasElement.width, _canvasElement.height);

    const width = 25;
    const height = 50;
    const xDrawOffset = 1;
    const yDrawOffset = 1;

    for (let x = 0; x <= width; x++) {
        // North-south lines
        _context.beginPath();
        _context.moveTo(x * _FLOOR_SIZE + xDrawOffset, yDrawOffset);
        _context.lineTo(x * _FLOOR_SIZE + xDrawOffset, height * _FLOOR_SIZE + yDrawOffset);
        _context.stroke();
    }

    for (let y = 0; y <= height; y++) {
        // East-West lines
        _context.beginPath();
        _context.moveTo(xDrawOffset, y * _FLOOR_SIZE + yDrawOffset);
        _context.lineTo(width * _FLOOR_SIZE + xDrawOffset, y * _FLOOR_SIZE + yDrawOffset);
        _context.stroke();
    }

    if (_imagesLoaded === _imagesToLoad) {
        //console.log("Draw board");

        for (let x = 0; x < width; x++) {
            for (let y = 0; y < height; y++) {
                let availableMovementSelection = false;
                let availableExtraMovementSelection = false;
                let selectedMovementSelection = false;
                for (let m = 0; m < game.availableMoves.length; m++) {
                    const move = game.availableMoves[m];
                    if (move.x === x && move.y === y) {
                        availableMovementSelection = true;
                        break;
                    }
                }

                for (let m = 0; m < game.availableExtraMoves.length; m++) {
                    const move = game.availableExtraMoves[m];
                    if (move.x === x && move.y === y) {
                        availableExtraMovementSelection = true;
                        break;
                    }
                }

                for (let m = 0; m < game.selectedMoves.length; m++) {
                    const move = game.selectedMoves[m];
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

        let selectedPlayerX = -1, selectedPlayerY = -1;
        let ballInPlayersHands = false;
        if (game.selectedPlayer !== null) {
            selectedPlayerX = game.selectedPlayer.boardPosition.x;
            selectedPlayerY = game.selectedPlayer.boardPosition.y;
        }

        for (let p = 0; p < game.homeTeam.players.length; p++) {
            const player = game.homeTeam.players[p];
            const x = player.boardPosition.x;
            const y = player.boardPosition.y;
            if (x !== -1) {
                const inAnimation = drawPlayer(_context, game.homeTeam, player, _IMAGE_PLAYER1, x, y, player.rotation, selectedPlayerX, selectedPlayerY);
                if (inAnimation === false && x === game.ball.boardPosition.x && y === game.ball.boardPosition.y) {
                    ballInPlayersHands = true;
                }
            }
        }

        for (let p = 0; p < game.visitorTeam.players.length; p++) {
            const player = game.visitorTeam.players[p];
            const x = player.boardPosition.x;
            const y = player.boardPosition.y;

            if (x !== -1) {
                const inAnimation = drawPlayer(_context, game.visitorTeam, player, _IMAGE_PLAYER1, x, y, player.rotation, selectedPlayerX, selectedPlayerY);
                if (inAnimation === false && x === game.ball.boardPosition.x && y == game.ball.boardPosition.y) {
                    ballInPlayersHands = true;
                }
            }
        }

        if (_animationBall !== undefined) {
            // Ball in animation
            if (_animationBall.startTime > 0) {
                // Static ball position since player has not yet reach this board position
                _context.drawImage(_images[_IMAGE_BALL_ANIMATION], _animationBall.position.x * _FLOOR_SIZE, _animationBall.position.y * _FLOOR_SIZE);
            }
            else if (_animationBall.rotation ===-1) {
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
        else if (game.ball.boardPosition.x !== -1) {
            // Static ball position
            const ballImage = ballInPlayersHands ? _images[_IMAGE_BALL] : _images[_IMAGE_BALL_ANIMATION];
            _context.drawImage(ballImage, game.ball.boardPosition.x * _FLOOR_SIZE, game.ball.boardPosition.y * _FLOOR_SIZE);
        }
    }

    _context.restore();
}

class DotNet {
    static invokeMethod: Function;
}

function calculatePosition(event: MouseEvent): void {

    if (!_animationRunning) {
        const x = Math.floor(event.offsetX / _FLOOR_SIZE);
        const y = Math.floor(event.offsetY / _FLOOR_SIZE);

        console.log("Clicked canvas at " + x + " - " + y);
        console.log(_game);

        DotNet.invokeMethod("UR.Client", "CanvasClickReceived", x, y);
    }
}

document.addEventListener("keydown", (event: KeyboardEvent) => {
    if (event.keyCode !== 123 /* F12 */) {
        event.preventDefault();
    }
});

document.addEventListener("keyup", (event: KeyboardEvent) => {
    if (event.keyCode !== 123 /* F12 */) {
        event.preventDefault();

        if (!_animationRunning) {
            DotNet.invokeMethod("UR.Client", "CanvasKeyUpReceived", event.keyCode);
        }
    }
});

window.addEventListener("scroll", function () {
    scrollElement();
});
