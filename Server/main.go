package main

import (
	// Standard Libraries
	"encoding/json"
	"fmt"
	"net/http"
	"os"
	"time"
  "crypto/sha256"

	// Websocket
	//"github.com/gorilla/websocket"

  // Config Directories
  "mylib/data"

	// DataBase
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)


// Func for Error test
func FastErroring(err error, txt string) {
  if err != nil {
    h, m, s := time.Now().Clock()
    fmt.Printf("%d:%d:%d|\n%s ==> %v\n",h,m,s , txt, err)
    http.NotFoundHandler()
  }
}

func main() {

  // JSON file reading and Formating
  readFile, err := os.ReadFile("ignored/db_guard.json")
  FastErroring(err, "MAIN: OS | Can't open and read file")

  var dbConf data.DBConfig
  err = json.Unmarshal(readFile, &dbConf)
  FastErroring(err, "MAIN JSON | Can't input JSON format into struct")


  // Connect to DB (DSN = Data Source Name)
  dsn := fmt.Sprintf("%s:%s@tcp(%s:%d)/%s?parseTime=true&timeout=5s", 
      dbConf.DBUser, dbConf.DBPass, dbConf.DBHost, dbConf.DBPort, dbConf.DBName,
  )
  db, err := sql.Open("mysql", dsn)
  FastErroring(err, "MAIN: DB | Connect error")
  err = db.Ping()
  FastErroring(err, "MAIN: DB | Ping error")
  fmt.Println("MAIN: DB | Connected!")
  defer fmt.Println("MAIN: DB | closed")
  defer db.Close()



  // Server Handler and Listener 
  mux := http.NewServeMux()

  mux.HandleFunc("POST /auth", func(w http.ResponseWriter, r *http.Request) {
    PostUserInfo(w, r, db)
  })
  // for profile settings and some other info
  mux.HandleFunc("GET /auth/{id}", func(w http.ResponseWriter, r *http.Request) {
    GetUserInfo(w, r, db)
  })
  mux.HandleFunc("GET /group/{id}", func(w http.ResponseWriter, r *http.Request) {
  })
  mux.HandleFunc("POST /group", func(w http.ResponseWriter, r *http.Request) {
      PostUserInfo(w, r, db)
  })
  mux.HandleFunc("GET /message/{id}", func(w http.ResponseWriter, r *http.Request) {
  })


  http.ListenAndServe(":8080", mux)
}



// user GET/POST
func PostUserInfo(w http.ResponseWriter, r *http.Request, db *sql.DB) {

  userPOST, err := db.Prepare("INSERT INTO user_t (Login, Password, GroupID) VALUES (?, ?, ?)")
  FastErroring(err, "POST: DB Prep | Error")
  defer fmt.Println("POST: DB Prepare | Closed")
  defer userPOST.Close()

  username := r.FormValue(data.Login)
  password := r.FormValue(data.Password)
  group := r.FormValue(data.Groupid)


  if username == "" || password == "" || group == "" {
    fmt.Printf("POST username %s\npassword %s\ngroup %s\n", username, password, group)
    w.Write([]byte("NULL"))
    return
  }

  // Hash function
  h := sha256.New()
  h.Write([]byte(password))
  hash := h.Sum(nil)

  res, err := userPOST.Exec(username, hash, group)
  FastErroring(err, "POST: Exec | Some Issues")
  if res == nil {
    fmt.Println("Test of NULL")
    w.Write([]byte("That Login already exists!"))
    return
  }
  w.Write([]byte("OK!"))

}

func GetUserInfo(w http.ResponseWriter, r *http.Request, db *sql.DB) {

  userid := r.PathValue(data.Id)
  if userid == "" {
    http.Error(w, "no ID", http.StatusBadRequest)
    return
  }
  
  fmt.Println(userid)

  rows, err := db.Query("SELECT Login, GroupID FROM user_t WHERE ID=?", userid)
  FastErroring(err, "GET: DB Query | Error")
  defer fmt.Println("GET: DB Prepare | Closed")
  defer rows.Close()

  //var users []UserJSON
  var us data.UserJSON
  for rows.Next() {
    if err = rows.Scan(&us.User, &us.GroupID); err != nil {
      FastErroring(err, "GET: DB Prep | Some Issues")
    }
    fmt.Println(us)
  }

  marsh, err := json.Marshal(us)
  FastErroring(err, "GET: JSON Marshal | Can't Marshal")
  w.Write(marsh)
  fmt.Println("GET was Sent")

}

func CreateMessage(r *http.Request, db *sql.DB) {
}


func CreateGroup(r *http.Request, db *sql.DB) {
}

