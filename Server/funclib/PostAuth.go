package funclib

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"time"

	// Libs
	"ml/data"

	// Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"

)

// AUTH POST; It is used for accepting HTTP POST Method [creating new account]
func PostAuth(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("PostAuth: Request Body Closed")
  defer r.Body.Close()

  // Getting data from our request
  username := r.FormValue(data.Login)
  password := r.FormValue(data.Password)
  group_id := r.FormValue(data.GroupID)

  // Checking if some requests are NULL
  if username == "" || password == "" || group_id == "" {
    log.Printf("username:|%s|\tpassword:|%s|\tgroup_id:|%s|\n", username, password, group_id)
    tmp := struct {
      Us    string  `json:"login"`
      Pass  string  `json:"password"`
      Gr    string  `json:"group_id"`
    }{
      Us: username,
      Pass: password,
      Gr: group_id,
    }
    w.WriteHeader(http.StatusBadRequest)
    enc := json.NewEncoder(w)
    if err := enc.Encode(&tmp); err != nil {
      w.WriteHeader(http.StatusInternalServerError)
      log.Printf("| PostAuth: Encoder | Error ==> %s\n", err)
      return
    }
    log.Println("JSON with nulls was sent")
    return
  }

  // Cheking if Username already exists
  ex, err := userCheck(username, db)
  if err != nil {
    log.Printf("| PostAuth: Username check | Error ==> %s\n", err)
    return
  }
  if ex {
    w.WriteHeader(http.StatusBadRequest)
    w.Write([]byte("USER EXISTS"))
    log.Println("User Exists")
    return
  }

  // Hash for password
  var hash_password []byte
  var psw Password = &passImpl{db: db, hash_ps: &hash_password}
  psw.Hash(password)

  // Insert data into DB
  cols := data.UserCols
  query := fmt.Sprintf(`
    INSERT INTO %s (%s, %s, %s, %s)
    VALUES (?, ?, ?, ?)
  `,
  data.User_Table, cols.Login, cols.Password, cols.GroupID, cols.Created)

  result, err := db.Exec(query, username, hash_password, group_id, time.Now())
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| PostAuth: DB Exec | Error ==> %s\n", err)
    return
  }
  rowAdd, err := result.RowsAffected()
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| PostAuth: Exec RowsAdded | Error ==> %s\n", err)
    return
  }
  if rowAdd<=0 {
    w.WriteHeader(http.StatusNoContent)
    log.Println("No content")
    return
  }

  // Automatically gives Token 
  var token []byte
  var tk Token = &tokenImpl{db: db, token: &token}
  err = tk.Create(username, hash_password)
  if err!=nil {
    InternalError(w, "| PostAuth: Token- Create | Error ==> %s\n", err)
    return
  }
  
  // Send token via JSON
  tmp := struct{ Token []byte `json:"token"`}{Token: token}
  marsh, err := json.Marshal(tmp)
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| PostAuth: JSON Marshal | Error ==> %s\n", err)
    return
  }

  log.Println("created")
  w.WriteHeader(http.StatusCreated)
  w.Write(marsh)

}

