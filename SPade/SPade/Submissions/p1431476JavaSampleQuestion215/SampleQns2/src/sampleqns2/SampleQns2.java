/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sampleqns2;

import java.util.Scanner;

/**
 *
 * @author tongliang
 */
public class SampleQns2 {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        System.out.println("Choose rental type: ");
        Scanner sc = new Scanner(System.in);

        int type = 0 ;
        try {
            type = sc.nextInt();
        } catch (Exception e) {
            System.out.println("Exception caught.");
        }

        double rate = 0;
        double hours = 0;

        switch (type) {
            case 1:
                System.out.println("Choose number of hours to rent: ");
                hours = sc.nextDouble();
                if (hours >= 3) {
                    rate += 5.5 * 0.7;
                } else {
                    rate += 5.5;
                }

                System.out.println("Rate is: " + rate);
                break;
            case 2:
                System.out.println("Choose number of hours to rent: ");
                hours = sc.nextDouble();
                if (hours >= 3) {
                    rate += 7.8 * 0.7;
                } else {
                    rate += 7.8;
                }

                System.out.println("Rate is: " + rate);
                break;
            default:
                System.out.println("Error. Please enter either 1 or 2.");
                break;
        }
    }//end of main method
}
