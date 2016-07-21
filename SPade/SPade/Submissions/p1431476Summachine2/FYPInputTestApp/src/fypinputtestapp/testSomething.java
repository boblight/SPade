/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package fypinputtestapp;

/**
 *
 * @author tongliang
 */
public class testSomething {
    
    public volatile static int i;
    
    public static void main(String args[]) {
        i = 1;
        
        new Thread(() -> {
            i = 2;
        }).start();
        
        System.out.println(i);
        
    }//end of main method
    
}
