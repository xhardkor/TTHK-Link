package funclib

import (
  "testing"
)

func TestToken(t *testing.T) {
  res, _ := GetToken(nil, "Pepe", []byte("asd"), nil)
  var not_exp []byte = nil

  if string(res) == string(not_exp) {
    t.Errorf("\nres: %s\nnot_exp: %s", res, not_exp)
  }
}
