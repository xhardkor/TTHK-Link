package main

import (
	// Standard Libraries
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"os"

	//"os/exec"

	// Websocket
	//"github.com/gorilla/websocket"

	// Config Directories
	"ml/data"
	"ml/funclib"

	// Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)



func main() {
  // JSON file reading and Formating
  readFile, err := os.ReadFile(data.DB_Dir)
  if err != nil {
    log.Printf("MAIN: OS Read File | Error ==> %s", err)
    return
  }

  var dbConf data.DBConfig
  if err := json.Unmarshal(readFile, &dbConf); err != nil {
    log.Printf("MAIN: JSON Unmarshal | Error ==> %s", err)
    return
  }


  // Connect to DB (DSN = Data Source Name)
  dsn := fmt.Sprintf("%s:%s@tcp(%s:%d)/%s?parseTime=true&timeout=5s", 
      dbConf.DBUser, dbConf.DBPass, dbConf.DBHost, dbConf.DBPort, dbConf.DBName,
  )
  db, err := sql.Open("mysql", dsn)
  if err != nil {
    log.Printf("MAIN: DB Connect | Error ==> %s", err)
    return
  }
  if err := db.Ping(); err != nil {
    log.Printf("MAIN: DB Ping | Error ==> %s", err)
    return
  }
  fmt.Println("MAIN: DB | Connected!")
  defer fmt.Println("MAIN: DB | closed")
  defer db.Close()



  //K: Server Handler and Listener 
  mux := http.NewServeMux()


//K: creating new auth
  mux.HandleFunc("POST /auth/create", func(w http.ResponseWriter, r *http.Request) {
    funclib.PostAuth(w, r, db)
  })
//K: give all info user has (w/o courses)[also it gives Token]
  mux.HandleFunc("GET /auth", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetUserInfo(w, r, db)
  })
  //WARN: give all Courses from a user(id)
  mux.HandleFunc("GET /user/courses", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetCourses(w, r, db)
  })
//NK: edit a message 
  mux.HandleFunc("PATCH /message/{group_id}/{course_id}", func(w http.ResponseWriter, r *http.Request) {
    funclib.EditMessage(w, r, db)
  })
//K: write a message 
  mux.HandleFunc("POST /message", func(w http.ResponseWriter, r *http.Request) {
    funclib.PostMessage(w,r,db)
  })
//NK: delete a message 
  mux.HandleFunc("DELETE /message/{id}", func(w http.ResponseWriter, r *http.Request) {
  })
//K: get messages
  mux.HandleFunc("GET /messages/{room_id}/{course_id}", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetMessages(w,r,db)
  })

//K: Server listen and serve
  err = http.ListenAndServe(":8080", mux)
  if err!=nil {
    log.Fatalf("Can't Start Server ==> %s\n", err)
    os.Exit(-1)
  }
}


