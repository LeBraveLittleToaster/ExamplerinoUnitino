export const tileType = [
    "RED",
    "GREEN",
    "BLUE"
]

export class GameBoard{
    constructor(boardXY){
        this.boardXY = boardXY;
    }

    getRandomPosition(){
        let posX = Math.floor(Math.random() * this.boardXY.length);
        let posY = Math.floor(Math.random() * this.boardXY[posX].length);
        return [posX, posY]
    }

    getGameBoard(){
        return this.boardXY;
    }
}

export class Tile {
    constructor(tileType){
        this.tileType = tileType
    }
}
