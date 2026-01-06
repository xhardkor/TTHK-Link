package funclib

import (
  "crypto/sha256"
  "fmt"
  "log"
  "time"

  // Libs
  "udlib/data"

  // Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)

// Alias for TokenChecker output values
type tokenGetter func(string, []byte)([]byte, error)
// Generates token for a current session
func GetToken(user string, psw []byte) (hash []byte, err error) {

  // Logging
  if user == "" || psw == nil{
    log.Printf("GetToken: Logging | Some NULL's")
    return nil, fmt.Errorf("NULL")
  }

  // Generates Token
  h := sha256.New()
  t := time.Now().Add(10 * time.Minute)
  h.Write([]byte(user + string(psw) + t.GoString() + data.Salt))
  hash = h.Sum(nil)
  return hash, nil
}

// Toker Checker 
func TokenChecker(user string, token []byte, db *sql.DB) (tokenGetter, bool) {

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

  row, err := db.Query(querry, user, token)
  if err != nil {
    log.Printf("TokenChecker: DB Query | Error ==> %s", err)
    return nil, false
  }
  defer fmt.Println("TokenChecker Row Closed")
  defer row.Close()

  if err := row.Scan(); err != nil {
    return GetToken, false
  }

  return nil, true

}

// Hash Password Generator
func CreateHashPsw(psw string) []byte {
  h := sha256.New()
  h.Write([]byte(psw + data.Salt))
  hash := h.Sum(nil)
  return hash
}

