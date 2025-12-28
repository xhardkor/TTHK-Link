package funclib

import (
	"fmt"
	"net/http"
	"time"

  	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)

// Func for Error test
func FastErroring(err error, txt string) (status bool) {
  if err != nil {
    h, m, s := time.Now().Clock()
    fmt.Printf("%d:%d:%d|\n%s ==> %v\n",h,m,s , txt, err)
    http.NotFoundHandler()
    return false
  }
  return true
}


// Func for DB id checking
func DB_Checking(db *sql.DB, sele, from, whr, imp_id string) (status bool) {
  querry := fmt.Sprintf("SELECT %s FROM %s WHERE %s=?", sele, from, whr)
  id := db.QueryRow(querry, imp_id)
  if id != nil {
    return false
  }
  return true
}
