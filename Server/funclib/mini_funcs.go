package funclib

import (
	"bytes"
	"crypto/sha256"
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

// internal error handler
func InternalError(w http.ResponseWriter, msg string, err error) {
  log.Printf(msg, err)
  http.Error(w, http.StatusText(http.StatusInternalServerError), http.StatusInternalServerError)
}



//// Interfaces

//MAIN: Interface for Tokens
type Token interface {
  Create(user string, hs_psw []byte) (null error)
  Check(user string) (exist bool)
  Get(r *http.Request) (decode error)
}
type tokenImpl struct{
  db *sql.DB
  token *[]byte
}

//F: Generates token for a current session using interface
func (t *tokenImpl) Create(user string, hs_psw []byte) (null error) {
  db := t.db

  // Logging
  if user == "" || hs_psw == nil{
    log.Printf("| Token- Create: Logging | Some NULL's\n")
    return fmt.Errorf("NULL VALUES")
  }

  // Generates Token
  h := sha256.New()
  tn := time.Now().Add(10 * time.Minute)
  h.Write([]byte(user + string(hs_psw) + tn.GoString() + data.Salt))
  token := h.Sum(nil)
  *t.token = token

  // Writing token into DB
  t_cols := data.TokenCols
  query := fmt.Sprintf(`
    INSERT INTO %s (%s, %s, %s)
    VALUES (?, ?, ?)
  `,
  data.Token_Table, t_cols.UserName, t_cols.Token, t_cols.Time)
  res, err := db.Exec(query, user, token, time.Now())
  if err != nil {
    return err
  }
  rowAdd, err := res.RowsAffected()
  if err != nil {
    return err
  }
  if rowAdd <=0 {
    return nil
  }

  return nil
}

//F: Checks token for a current session using interface
func (t *tokenImpl) Check(user string) (exist bool) {

  db := t.db
  t_cols := data.TokenCols
  u_cols := data.UserCols

  querry := fmt.Sprintf(`
    SELECT t.%s FROM %s AS t
    JOIN %s AS u ON t.%s=u.%s
    WHERE t.%s=? AND t.%s=?
  `,
  t_cols.Time, data.Token_Table,
  data.User_Table, t_cols.UserName, u_cols.Login,
  t_cols.UserName, t_cols.Token)

  ////XXX: NOT FINISHED
  var tokenTime string
  if err := db.QueryRow(querry, user, *t.token).Scan(&tokenTime); err != nil {
    log.Printf("| Token- Check: DB Query | Error ==> %s\n", err)
    fmt.Println(tokenTime)
    return false
  }
  fmt.Println(tokenTime)

  return true
}

//F: Get Token from the Body of the Request 
func (t *tokenImpl) Get(r *http.Request) (decode error) {

  var tok struct{Token []byte `json:"token"`}
  if err := json.NewDecoder(r.Body).Decode(&tok); err!=nil {
    return err
  }
  *t.token = tok.Token
  return nil
}




//MAIN: Interface for Passwords
type Password interface {
  Hash(psw string)
  Check(user string, hs_psw []byte) (ok bool, err error)
}
type passImpl struct {
  db *sql.DB
  msg *string
  hash_ps *[]byte
}

//F: Hash Password Generator
func (ps *passImpl) Hash(psw string) {
  h := sha256.New()
  h.Write([]byte(psw + data.Salt))
  *ps.hash_ps = h.Sum(nil)
}

//F: Checking if password is right
func (ps *passImpl) Check(user string, hs_psw []byte) (ok bool, err error) {
  db := ps.db

  u_cols := data.UserCols
  query := fmt.Sprintf("SELECT %s FROM %s WHERE %s=?", u_cols.Password, data.User_Table, u_cols.Login)

  var hash []byte
  if err:=db.QueryRow(query, user).Scan(&hash); err==sql.ErrNoRows {
    *ps.msg = "Invalid log or pass |sql"
    return false, nil
  } else if err!=nil{
    return false, err
  }
  if !bytes.Equal(hash, hs_psw) {
    *ps.msg = "Invalid log or pass |compare"
    return false, nil
  }
  return true, nil
}

//F: Checking if username already exist
func userCheck(user string, db *sql.DB) (exist bool, err error) {

  u_cols := data.UserCols
  query := fmt.Sprintf(`
    SELECT 1 FROM %s
    WHERE %s=?
  `, data.User_Table, u_cols.Login)
  
  if err := db.QueryRow(query, user).Scan(new(int)); err==sql.ErrNoRows {
    return false, nil
  } else if err!=nil {
    return false, err
  }
  return true, nil

}




//// Testing

type TestT interface {
  Trunc()
}
type testImpl struct {
  db *sql.DB
}

// Test function Truncate token_t
func (t testImpl) Trunc() {
  db := t.db
  query := fmt.Sprintf("TRUNCATE TABLE %s", data.Token_Table)
  res, _ := db.Exec(query)
  fmt.Println("Truncated", res)
}
