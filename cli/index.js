import chalk from 'chalk';
import inquirer from 'inquirer';
import chalkAnimation from 'chalk-animation';
import { createSpinner } from 'nanospinner';
import { GameBoard, Tile, tileType } from './gameboard.js';
import {WebSocketServer} from 'ws';
import WebSocket from 'ws';


const MESSAGE_TYPE_INIT = "INIT";
const MESSAGE_TYPE_MOVE = "MOVE";


let tiles = []
for (let x = 0; x < 5; x++) {
    let subTiles = []
    for (let y = 0; y < 5; y++) {
        subTiles.push(new Tile(tileType[Math.floor(Math.random() * 3)]));
    }
    tiles.push(subTiles);
}



const gb = new GameBoard(tiles);

const sleep = (ms = 1000) => new Promise((r) => setTimeout(r, ms));

async function welcome() {
    const rainbowTitle = chalkAnimation.rainbow(
        'Welcome to the demo server \n'
    );

    await sleep(2000);
    rainbowTitle.stop();

    console.log(`
        ${chalk.bgBlue(' How to play ')} 
        You can choose if you wanna send player1 or player2 to a random point on the gameboard...
    `);
}





const ws = new WebSocketServer({port  : 1324});

function createStartUpMessage(gameboard){
    return JSON.stringify({
        "msgType" : MESSAGE_TYPE_INIT,
        "tiles" : gameboard.getGameBoard()
    });
}

function createMoveMessage(posX, posY, playername){
    return JSON.stringify({
        "msgType" : MESSAGE_TYPE_MOVE,
        "x" : posX,
        "y" : posY,
        "player" : playername
    });
}

async function questionPlayer() {
    const answers = await inquirer.prompt({
        name: 'question',
        type: 'list',
        message: 'Which player you wanna send?\n',
        choices: [
            "p1",
            "p2"
        ],
    });

    return handleAnswer(answers.question);
}

async function handleAnswer(playername) {
    const spinner = createSpinner('Checking player...').start();

    await sleep(1000);
    let rPos = gb.getRandomPosition();

    ws.clients.forEach(function each(client) {
        if (client.readyState === WebSocket.OPEN) {
          client.send(createMoveMessage(rPos[0], rPos[1], playername));
        }
      });
    spinner.success({ text: "Sending " + chalk.bgGreen(playername) + " to x=" + chalk.bgRed(rPos[0]) + " and y=" + chalk.bgRed(rPos[1]) });
    
}

ws.on('connection', (connection) => {
    connection.send(createStartUpMessage(gb));
});

console.clear();
await welcome();
while (true) {
    await questionPlayer();
}
