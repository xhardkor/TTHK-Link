package main

import (
	// Standard Libraries
	"encoding/json"
	"fmt"
	"net/http"
	"os"
	//"os/exec"

	// Websocket
	//"github.com/gorilla/websocket"

	// Config Directories
	"udlib/data"
  "udlib/funclib"

	// Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)



func main() {
  // JSON file reading and Formating
  readFile, err := os.ReadFile(data.DB_Dir)
  funclib.FastErroring(err, "MAIN: OS | Can't open and read file")

  var dbConf data.DBConfig
  err = json.Unmarshal(readFile, &dbConf)
  funclib.FastErroring(err, "MAIN: JSON | Can't input JSON format into struct")


  // Connect to DB (DSN = Data Source Name)
  dsn := fmt.Sprintf("%s:%s@tcp(%s:%d)/%s?parseTime=true&timeout=5s", 
      dbConf.DBUser, dbConf.DBPass, dbConf.DBHost, dbConf.DBPort, dbConf.DBName,
  )
  db, err := sql.Open("mysql", dsn)
  funclib.FastErroring(err, "MAIN: DB | Connect error")
  err = db.Ping()
  funclib.FastErroring(err, "MAIN: DB | Ping error")
  fmt.Println("MAIN: DB | Connected!")
  defer fmt.Println("MAIN: DB | closed")
  defer db.Close()



  // Server Handler and Listener 
  mux := http.NewServeMux()

  // creating new auth
  mux.HandleFunc("POST /auth", func(w http.ResponseWriter, r *http.Request) {
    funclib.PostAuth(w, r, db)
  })
  // give id and token(-)
  mux.HandleFunc("GET /auth", func(w http.ResponseWriter, r *http.Request) {
  })
  // give all groups which user has
  mux.HandleFunc("GET /user/{id}", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetUserInfo(w, r, db)
  })
  // give all messages from a group(id)
  mux.HandleFunc("GET /group/{id}", func(w http.ResponseWriter, r *http.Request) {
  })
  // create a new group
  mux.HandleFunc("POST /group", func(w http.ResponseWriter, r *http.Request) {
  })
  // write a message 
  mux.HandleFunc("POST /message/{id}", func(w http.ResponseWriter, r *http.Request) {
  })


  http.ListenAndServe(":8080", mux)
}


