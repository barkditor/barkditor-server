package main

import (
	"flag"
	"fmt"
	"log"
	"net"
	"net/rpc"
	"net/rpc/jsonrpc"
	"os"

	"github.com/barkditor/barkditor-server/api"
)

var port = flag.Int("port", 5000, "Port to start server on")

func main() {
	*log.Default() = *log.New(
		os.Stdout,
		" [ server ] ",
		log.LstdFlags|log.Lshortfile|log.Lmsgprefix,
	)

	rpc.Register(api.API{})

	l, err := net.Listen("tcp", fmt.Sprintf(":%v", *port))
	if err != nil {
		log.Fatalln(err)
	}

	log.Printf("Listening on port %v\n", *port)
	for {
		conn, err := l.Accept()
		if err != nil {
			netErr := err.(net.Error)
			if netErr.Timeout() {
				continue
			}

			log.Fatalln(err)
		}

		log.Printf("Connection from %v\n", conn.RemoteAddr().String())

		go rpc.ServeCodec(jsonrpc.NewServerCodec(conn))
	}
}
