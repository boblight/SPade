/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package fyptestapp;

/**
 *
 * @author tongliang
 */
public class FYPTestApp {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        String msg = "";

        for (int i = 0; i < 10; i++) {
            if (i <= 4) {
                for (int j = 0; j < i + 1; j++) {
                    msg += "*";
                }
            } else {
                for (int j = 10; j >= i + 1; j--) {
                    msg += "*";
                }
            }
            msg += "\r\n";
        }

        System.out.println(msg);
    }//end of main method

}
