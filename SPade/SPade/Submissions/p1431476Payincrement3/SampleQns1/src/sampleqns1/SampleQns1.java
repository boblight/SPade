/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sampleqns1;

import java.util.Scanner;

/**
 *
 * @author tongliang
 */
public class SampleQns1 {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        System.out.println("Enter years of service: ");
        
        Scanner sc = new Scanner(System.in);
        int i = sc.nextInt();
        
        System.out.println("Enter salary: ");
        int j = sc.nextInt();
        
        int inc;
        
        if(i < 10) {
            if(j < 1000) {
                inc = 100;
            } else if (j < 2000) {
                inc = 200;
            } else {
                inc = 300;
            }
        } else {
            if(j < 1000) {
                inc = 200;
            } else if (j < 2000) {
                inc = 300;
            } else {
                inc = 400;
            }
        }
        
        System.out.println("Increment for staff is: " + inc);
    }//end of main class

}
